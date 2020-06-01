import sys


class Camera:
    __WIDTH = 640
    __HEIGHT = 480

    def __init__(self, racecar):
        self.__racecar = racecar

    def get_width(self):
        return self.__WIDTH

    def get_height(self):
        return self.__HEIGHT
