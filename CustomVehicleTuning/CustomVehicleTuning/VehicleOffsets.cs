using System;
using System.Collections.Generic;
using System.Diagnostics;
using GTA;

namespace CustomVehicleTuning
{
    public class VehicleOffset
    {
        // Getters
        public static unsafe float GetTractionCurveMax(Vehicle vehicle)
        {
            return *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.TractionCurveMax);
        }
        public static unsafe float GetInitialDriveMaxFlatVel(Vehicle vehicle)
        {
            return *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.InitialDriveMaxFlatVel);
        }
        public static unsafe float GetDriveMaxFlatVel(Vehicle vehicle)
        {
            return *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.DriveMaxFlatVel);
        }
        public static unsafe float GetFinalDrive(Vehicle vehicle)
        {
            return *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.FinalDrive);
        }
        public static unsafe float GetClutchShiftUp(Vehicle vehicle)
        {
            return *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.ClutchShiftUp);
        }
        public static unsafe float GetClutchShiftDown(Vehicle vehicle)
        {
            return *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.ClutchShiftDown);
        }
        public static unsafe float GetDriveForce(Vehicle vehicle)
        {
            return *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.DriveForce);
        }
        public static unsafe int GetBrakeBiasFront(Vehicle vehicle)
        {
            return *(int*)(((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.Handling) + (ulong)VehicleOffsetEnum.BrakeBiasFront);
        }
        public static unsafe int GetBrakeBiasRear(Vehicle vehicle)
        {
            return *(int*)(((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.Handling) + (ulong)VehicleOffsetEnum.BrakeBiasRear);
        }
        public static unsafe float GetClutch(Vehicle vehicle)
        {
            return *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.Clutch);
        }
        public static unsafe float GetTurbo(Vehicle vehicle)
        {
            return *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.Turbo);
        }
        public static unsafe short GetGear(Vehicle vehicle)
        {
            return *(short*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.CurrentGear);
        }
        public static unsafe short GetTopGear(Vehicle vehicle)
        {
            return *(short*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.TopGear);
        }
        public static unsafe float[] GetGearRatios(Vehicle vehicle)
        {
            ushort numGears = 8;
            float[] ratios = new float[numGears];
            for (ushort i = 0; i < numGears; i++)
            {
                ratios[i] = *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.GearRatios + i * (ulong)sizeof(float));
            }
            return ratios;
        }
        public static unsafe float GetGearRatio(Vehicle vehicle, int gear)
        {
            return GetGearRatios(vehicle)[gear];
        }
        public static unsafe float GetBrakePower(Vehicle vehicle)
        {
            return *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.BrakePower);
        }

        // Setters
        public static unsafe void SetTractionCurveMax(Vehicle vehicle, float value)
        {
            *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.TractionCurveMax) = value;
        }
        public static unsafe void SetInitialDriveMaxFlatVel(Vehicle vehicle, float value)
        {
            *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.InitialDriveMaxFlatVel) = value;
        }
        public static unsafe void SetDriveMaxFlatVel(Vehicle vehicle, float value)
        {
            *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.DriveMaxFlatVel) = value;
        }
        public static unsafe void SetFinalDrive(Vehicle vehicle, float value)
        {
            *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.FinalDrive) = value;
        }
        public static unsafe void SetClutchShiftUp(Vehicle vehicle, float value)
        {
            *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.ClutchShiftUp) = value;
        }
        public static unsafe void SetClutchShiftDown(Vehicle vehicle, float value)
        {
            *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.ClutchShiftDown) = value;
        }
        public static unsafe void SetDriveForce(Vehicle vehicle, float value)
        {
            *(float*)((ulong)GetHandlingPointer(vehicle) + (ulong)VehicleOffsetEnum.DriveForce) = value;
        }
        public static unsafe void SetBrakeBiasFront(Vehicle vehicle, int value)
        {
            *(int*)(((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.Handling) + (ulong)VehicleOffsetEnum.BrakeBiasFront) = value;
        }
        public static unsafe void SetBrakeBiasRear(Vehicle vehicle, int value)
        {
            *(int*)(((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.Handling) + (ulong)VehicleOffsetEnum.BrakeBiasRear) = value;
        }
        public static unsafe void SetClutch(Vehicle vehicle, float value)
        {
            *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.Clutch) = value;
        }
        public static unsafe void SetTurbo(Vehicle vehicle, float value)
        {
            *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.Turbo) = value;
        }
        public static unsafe void SetTopGear(Vehicle vehicle, short value)
        {
            *(short*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.TopGear) = value;
        }
        public static unsafe void SetGearRatios(Vehicle vehicle, float[] values)
        {
            for (ushort i = 0; i < values.Length; i++)
            {
                *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.GearRatios + i * (ulong)sizeof(float)) = values[i];
            }
        }
        public static unsafe void SetGearRatio(Vehicle vehicle, float value, int gear)
        {
            float[] ratios = GetGearRatios(vehicle);
            ratios[gear] = value;
            SetGearRatios(vehicle, ratios);
        }
        public static unsafe void SetBrakePower(Vehicle vehicle, float value)
        {
            *(float*)((ulong)vehicle.MemoryAddress + (ulong)VehicleOffsetEnum.BrakePower) = value;
        }
        // Handling Power
        private static unsafe dynamic GetHandlingPointer(Vehicle vehicle)
        {
            var address = vehicle.MemoryAddress;
            return *(ulong*)((ulong) address + (ulong) VehicleOffsetEnum.Handling);
        }
    }

    public enum VehicleOffsetEnum
    {
        DriveForce = 0x0054,
        ClutchShiftUp = 0x0058,
        ClutchShiftDown = 0x005C,
        FinalDrive = 0x0060,
        InitialDriveMaxFlatVel = 0x85C,
        DriveMaxFlatVel = 0x860,
        BrakeBiasFront = 0x0074,
        BrakeBiasRear = 0x0078,
        TractionCurveMax = 0x008C,
        CurrentGear = 0x832,
        TopGear = 0x836,
        GearRatios = 0x838,
        Clutch = 0x870,
        Throttle = 0x874,
        Turbo = 0x888,
        Handling = 0x8C8,
        BrakePower = 0x950
    }
}
