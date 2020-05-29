import sys


class Camera:
    def __init__(self, racecar):
        self.__racecar = racecar

    def get_width(self):
        self.__racecar._Racecar__send_call(self.__racecar.FunctionCode.camera_get_width)
        return int.from_bytes(self.__racecar._Racecar__receive_data(), sys.byteorder)

    def get_height(self):
        self.__racecar._Racecar__send_call(
            self.__racecar.FunctionCode.camera_get_height
        )
        return int.from_bytes(self.__racecar._Racecar__receive_data(), sys.byteorder)
