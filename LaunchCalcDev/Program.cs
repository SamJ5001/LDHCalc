﻿using System;


namespace LaunchCalcDev
{

    // Enter dates here for program testing - Format YYYY, MM, DD.
    // This program will assume default unless otherwise mentioned

    public static class dataEntry
    {

        // So I need to capture DATES in here, and then convert these to int distance types in the number crunch section.
        public static bool dataEntryinCode = true;

        /*
        internal static int dELockIMF = 30;
        internal static int dEiMFDue = 30;
        internal static int dEdueLaunch = 30;
        internal static bool dEchildCasting = false;
        internal static bool dEpldlRequred = false;
        internal static bool dEcmWorkflow = false;
        */


        // ENTER DATES HERE
        internal static DateTime inputLOCK = new DateTime(2021, 04, 16);
        internal static DateTime inputIMF = new DateTime(2021, 05, 28);

        // I'll define which one this goes to based upon enum selection
        internal static DateTime inputME = new DateTime(2021, 06, 04);

        // False = ME0, True = ME1 ("Tick this box for M&E Centric timeline")
        internal static bool inputMEWORKFLOW = false;


    }

    public enum contentType
    {
        feature,
        series
    }
    public enum sourceLang
    {
        English,
        Other
    }
    public enum MEQCWorkflow
    {
        // WFO = Standard, Post-IMF Delivery QC
        WF0,
        // WF1 = M&E Centric Timeline
        WF1
    }
    public enum QCType
    {
        Premix,
        Postmix,
        CQC
    }


    class Program
    {
        static void Main(string[] args)

        {
            Data data = new Data();

            data.setupData(data);
        }

        public static void Output(DateTime green, DateTime yellow, DateTime red)
        {
            Console.WriteLine("Recommended Launch Dates as follows:");
            Console.WriteLine($"Green: {green.ToShortDateString()}");
            Console.WriteLine($"Yellow: {yellow.ToShortDateString()}");
            Console.WriteLine($"Red: {red.ToShortDateString()}");
        }
    }


    class Data
    {
        // Input Data (interim storage in this class - pulled at the top from static entry class
        private int lockIMF;
        private int iMFDue;
        private int dueLaunch;

        private DateTime lastLockDate;

        private int lockIMFDef = 30;
        private int IMFDueDef = 30;
        private int DueLaunchDef = 30;

        float yellowFactor;
        float redFactor;


        internal int totalTimeline;


        // Output Data
        internal DateTime greenOut;
        internal DateTime yellowOut;
        internal DateTime redOut;



        public void setupData(Data dataClass)

        {

            if (dataEntry.dataEntryinCode == true)
            {
                pullStaticData();
            }
            else
            //

            {
                if (lastLockDate == DateTime.MinValue) { setDefaultsErrorDT(lastLockDate, "lastLockDate"); }

                if (lockIMF == 0) { lockIMF = setDefaultsErrorInt(lockIMF, "lockIMF"); }
                if (iMFDue == 0) { iMFDue = setDefaultsErrorInt(iMFDue, "iMFDue"); }
                if (dueLaunch == 0) { dueLaunch = setDefaultsErrorInt(dueLaunch, "dueLaunch"); }

            }
            // Folding down this error check, as I'm going to be taking all of this in for code until I'm half done anyway. I'll need to set up some data-type agnostic error check.


            // SO I need a method here to convert those dates to timelines. I need to calculate these differentials between 


            //Crunch Timeline returns an int value of days between kick-off and ideal launch, by summing the timelines provided and distorting/manimpulating them with project factors

            totalTimeline = Calculator.crunchTimeline(DateTime.Today, lockIMF, iMFDue, dueLaunch);



            // Set Yellow / Red Offsets
            yellowFactor = (dueLaunch * 0.75f);
            redFactor = (dueLaunch * 0.5f);

            // Send values to crunch
            greenOut = LightSum(0, totalTimeline);
            yellowOut = LightSum(1, totalTimeline);
            redOut = LightSum(2, totalTimeline);
            Program.Output(greenOut, yellowOut, redOut);
        }


        // The thing I'm multiplying back with for basic testing at this point. This is not realistic and would require a proportional offset of date / launch.




        DateTime LightSum(int light, int timeline)
        {
            var date = new DateTime();

            if (light == 0)
            {
                date = lastLockDate.AddDays(timeline);

                return date;
            }
            else if (light == 1)
            {
                date = lastLockDate.AddDays(timeline * yellowFactor);
                return date;
            }
            else if (light == 2)
            {
                date = lastLockDate.AddDays(timeline * redFactor);
                return date;
            }
            else
            {
                Console.WriteLine("ERROR; Unable to determine datetime.");
                return DateTime.MinValue;
            }


        }

        public void pullStaticData()
        {

            // Check booleans for casting etc here? This is where I take in the manual inputs which we can assume to be defaults for now.

            /*
             * 
             * lastLockDate = dataEntry.dELastLockDate;
            lockIMF = dataEntry.dELockIMF;
            iMFDue = dataEntry.dEiMFDue;
            dueLaunch = dataEntry.dEdueLaunch;
            */
        }




        int setDefaultsErrorInt(int defInt, string defSt)
        {
            // Leaving these all at 30 for now -- remove and pass through once necessary.
            defInt = 30;
            Console.WriteLine($"Setting date for {defSt} to default ({defInt}) for debugging. Please ensure all dates have filled values");
            return defInt;
        }
        DateTime setDefaultsErrorDT(DateTime defDT, string defSt)
        {
            // Leaving these all at 30 for now -- remove and pass through once necessary.
            defDT = DateTime.MinValue;
            Console.WriteLine($"Setting date for {defDT} to default ({DateTime.MinValue}) for debugging. Please ensure all dates have filled values");
            return defDT;
        }
    }


    static class Calculator
    {

        public static int crunchTimeline(DateTime today, int locktoIMF, int iMFtoDue, int duetoLaunch)
        {
            // This is where I'm gonna put the more actual mathy stuff.

            int timelineSummed = locktoIMF + iMFtoDue + duetoLaunch;

            return timelineSummed;

        }
    }
}