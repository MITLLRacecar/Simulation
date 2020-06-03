import cv2 as cv

class Display:
    def show_image(self, image):
        cv.namedWindow("display window", cv.WINDOW_NORMAL)
        cv.imshow("display window", image)
        cv.waitKey(1) # TODO is this line necessary?
