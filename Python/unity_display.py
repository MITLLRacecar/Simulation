import cv2 as cv

class Display:
    def show_image(self, image):
        cv.imshow("display window", image)
        cv.waitKey(1) # TODO is this line necessary?
