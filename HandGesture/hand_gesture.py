# organize imports
import cv2
import imutils
import numpy as np
from sklearn.metrics import pairwise
import time
from unity_socket import UnitySocket


class HandGestureRecognizer:
    def __init__(self):
        self.bg = None
        self.accumWeight = 0.5
        self.camera = cv2.VideoCapture(0)
        self.time_thresh = 2
        self.radius_Percent = 0.7

    def run(self):
        # inital our socket server to interact with our game
        my_socket = UnitySocket()

        # region of interest (ROI) coordinates
        top, right, bottom, left = 10, 350, 225, 590

        # initialize num of frames
        num_frames = 0

        # calibration indicator
        calibrated = False

        seen_hand = False
        last_fingers = 0
        last_sec = -1
        # keep looping, until interrupted
        while (True):
            # get the current frame
            (grabbed, frame) = self.camera.read()

            # resize the frame
            frame = imutils.resize(frame, width=700)

            # flip the frame so that it is not the mirror view
            frame = cv2.flip(frame, 1)

            # clone the frame
            clone = frame.copy()

            # get the ROI
            roi = frame[top:bottom, right:left]

            # convert the roi to grayscale and blur it
            gray = cv2.cvtColor(roi, cv2.COLOR_BGR2GRAY)
            gray = cv2.GaussianBlur(gray, (7, 7), 0)

            # to get the background, keep looking till a threshold is reached
            # so that our weighted average model gets calibrated
            if num_frames < 30:
                self.run_avg(gray, self.accumWeight)
                if num_frames == 1:
                    print("[STATUS] please wait! calibrating...")
                elif num_frames == 29:
                    print("[STATUS] calibration successfull...")
            else:
                # segment the hand region
                hand = self.segment(gray)

                # check whether hand region is segmented
                if hand is not None:
                    # if yes, unpack the thresholded image and
                    # segmented region
                    (thresholded, segmented) = hand

                    # draw the segmented region and display the frame
                    cv2.drawContours(clone, [segmented + (right, top)], -1, (0, 0, 255))

                    # count the number of fingers
                    fingers = self.count(thresholded, segmented)

                    # Checks whether the player holds their hand with same fingers for a fixed time
                    # If a hand is not seen in the box, the timer is restarted
                    if not seen_hand:
                        seen_hand = True
                        last_fingers = fingers
                        start_time = time.time()
                        print("Hand Detected, Timer Started\n")
                    elif seen_hand:
                        # Each time a hand is detected the number of fingers is checked
                        # If the number of fingers is changed, timer will reset
                        # Else the timer will count until the threshold is reached
                        if fingers == last_fingers:
                            temp_time = time.time()
                            temp_time = int(temp_time - start_time)
                            if last_sec == -1:
                                last_sec = temp_time
                            if temp_time == self.time_thresh:
                                # The number of fingers have remained constant for time_thresh seconds
                                # The result will be sent to the game by socket
                                print("Confirmed\n")
                                # send detected number to our game
                                my_socket.send_data(str(fingers))
                                seen_hand = False
                                last_fingers = 0
                                last_sec = -1
                            else:
                                if temp_time == last_sec + 1:
                                    print("Still need " + str(temp_time) + " seconds\n")
                                    last_sec = temp_time
                        else:
                            print("Fingers have been changed")
                            seen_hand = False
                            last_fingers = 0
                            last_sec = -1

                    cv2.putText(clone, str(fingers), (70, 45), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)

                    # show the thresholded image
                    cv2.imshow("Thesholded", thresholded)
                else:
                    print("Hand Removed")
                    seen_hand = False
                    last_fingers = 0
                    last_sec = -1
            # draw the segmented hand
            cv2.rectangle(clone, (left, top), (right, bottom), (0, 255, 0), 2)

            # increment the number of frames
            num_frames += 1

            # display the frame with segmented hand
            cv2.imshow("Video Feed", clone)

            # observe the keypress by the user
            keypress = cv2.waitKey(1) & 0xFF

            # if the user pressed "q", then stop looping
            if keypress == ord("q"):
                break
            if keypress == ord("c"):
                num_frames = 0

        # free up memory
        self.camera.release()
        cv2.destroyAllWindows()

    # --------------------------------------------------
    # To find the running average over the background
    # --------------------------------------------------
    def run_avg(self, image, accumWeight):
        if self.bg is None:
            self.bg = image.copy().astype("float")
            return
        cv2.accumulateWeighted(image, self.bg, accumWeight)

    # ---------------------------------------------
    # To segment the region of hand in the image
    # ---------------------------------------------
    def segment(self, image, threshold=25):
        diff = cv2.absdiff(self.bg.astype("uint8"), image)
        thresholded = cv2.threshold(diff, threshold, 255, cv2.THRESH_BINARY)[1]
        (cnts, _) = cv2.findContours(thresholded.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        if len(cnts) == 0:
            return
        else:
            segmented = max(cnts, key=cv2.contourArea)
            return (thresholded, segmented)

    # --------------------------------------------------------------
    # To count the number of fingers in the segmented hand region
    # --------------------------------------------------------------
    def count(self, thresholded, segmented):

        # Calculate the convex hull of the segmented region which is hand
        chull = cv2.convexHull(segmented)

        #Extract the extreme point of the convex hull and calculate the center
        extreme_top = tuple(chull[chull[:, :, 1].argmin()][0])
        extreme_bottom = tuple(chull[chull[:, :, 1].argmax()][0])
        extreme_left = tuple(chull[chull[:, :, 0].argmin()][0])
        extreme_right = tuple(chull[chull[:, :, 0].argmax()][0])

        cX = int((extreme_left[0] + extreme_right[0]) / 2)
        cY = int((extreme_top[1] + extreme_bottom[1]) / 2)

        distance = \
        pairwise.euclidean_distances([(cX, cY)], Y=[extreme_left, extreme_right, extreme_top, extreme_bottom])[0]
        maximum_distance = distance[distance.argmax()]

        radius = int(self.radius_Percent * maximum_distance)

        circumference = (2 * np.pi * radius)

        circular_roi = np.zeros(thresholded.shape[:2], dtype="uint8")

        # Draw a circle and bitwise and it with the segmented region to gather the collision contours
        cv2.circle(circular_roi, (cX, cY), radius, 255, 1)

        circular_roi = cv2.bitwise_and(thresholded, thresholded, mask=circular_roi)

        (cnts, _) = cv2.findContours(circular_roi.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)

        count = 0

        # Filter the contours to extract the correct finger numbers
        for c in cnts:
            (x, y, w, h) = cv2.boundingRect(c)
            if ((cY + (cY * 0.25)) > (y + h)) and ((circumference * 0.25) > c.shape[0]):
                count += 1

        return count

