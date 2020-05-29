import struct


class Drive:
    def __init__(self, racecar):
        self.__racecar = racecar

    def set_speed_angle(self, speed, angle):
        self.__racecar._Racecar__send_data(
            struct.pack(
                "Bff",
                self.__racecar.FunctionCode.drive_set_speed_angle.value,
                speed,
                angle,
            )
        )

    def stop(self):
        self.__racecar._Racecar__send_call(self.__racecar.FunctionCode.drive_stop)

    def set_max_speed_scale_factor(self, scale_factor):
        pass
