using System;


namespace LaunchCalcDev
{

    // Enter dates here for program testing - Format YYYY, MM, DD.
    // This program will assume default unless otherwise mentioned

    // I'm treating this as a feature. Then once we move to multiple episodes, we spawn separate data classes per ep as calculated by the program.

    public static class dataEntry
    {

        // So I need to capture DATES in here, and then convert these to int distance types in the number crunch section.
        public static bool dataEntryinCode = true;

        internal static bool dEchildCasting = false;
        internal static bool dEpldlRequred = false;
        internal static bool dEcmWorkflow = false;


        // ENTER DATES HERE
        internal static DateTime inputLOCK = new DateTime(2021, 04, 16);
        internal static DateTime inputIMF = new DateTime(2021, 05, 28);
        internal static DateTime inputDue = new DateTime(2021, 07, 28);

        internal static DateTime inputME0 = new DateTime(2021, 06, 04);
        internal static DateTime inputME1 = new DateTime(2021, 05, 14);

        // False = ME0, True = ME1 ("Tick this box for M&E Centric timeline")
        internal static bool inputMEWORKFLOW = false;
    }

    public enum ContentType
    {
        feature,
        series
    }
    public enum SourceLang
    {
        English,
        Other
    }
    public enum MEQCWorkflow
    {
        // WFO = Standard, Post-IMF Delivery QC
        WF0,
        // WF1 = M&E Centric Timeline
        WF1,
        // Debug(?) to stick with just the core AV asseets. Could be useful for licesned / PM-only workflows
        NoWorkflow
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

            data.setEnums();
            data.setupData(data);
        }

        public static void Output(DateTime dueDate, DateTime green, DateTime yellow, DateTime red)
        {
            Console.WriteLine($"Estimated Delivery Date for Service Assets: {dueDate.ToShortDateString()} ");

            Console.WriteLine("Recommended Launch Dates as follows:");
            Console.WriteLine($"Green: {green.ToShortDateString()}");
            Console.WriteLine($"Yellow: {yellow.ToShortDateString()}");
            Console.WriteLine($"Red: {red.ToShortDateString()}");
        }
    }


    class Data
    {
        public static ContentType contentType;
        public static SourceLang sourceLang;
        public static MEQCWorkflow mEQCWorkflow;
        public static QCType qCType;

        // Input Data (interim storage in this class - pulled at the top from static entry class
        private int lockIMF;
        private int iMFDue;
        private int dueLaunch;

        private int lockME1;
        private int mE1IMF;

        private int iMFME0;
        private int mE0Due;

        private DateTime lockDate;
        private DateTime iMFDate;
        private DateTime dueDate;
        private DateTime mE0Date;
        private DateTime mE1Date;

        float yellowFactor;
        float redFactor;


        internal int totalTimeline;


        // Output Data
        internal DateTime greenOut;
        internal DateTime yellowOut;
        internal DateTime redOut;

        public void setEnums()
        {
            switch (contentType)
            {
                case ContentType.feature:
                    contentType = ContentType.feature;
                    break;
                case ContentType.series:
                    contentType = ContentType.series;
                    break;
            }
            switch (sourceLang)
            {
                case SourceLang.English:
                    sourceLang = SourceLang.English;
                    break;
                case SourceLang.Other:
                    sourceLang = SourceLang.Other;
                    break;
            }
            switch (mEQCWorkflow)
            {
                case MEQCWorkflow.WF0:
                    mEQCWorkflow = MEQCWorkflow.WF0;
                    break;
                case MEQCWorkflow.WF1:
                    mEQCWorkflow = MEQCWorkflow.WF1;
                    break;
                case MEQCWorkflow.NoWorkflow:
                    mEQCWorkflow = MEQCWorkflow.NoWorkflow;
                    break;
            }
            switch (qCType)
            {
                case QCType.Premix:
                    qCType = QCType.Premix;
                    break;
                case QCType.Postmix:
                    qCType = QCType.Postmix;
                    break;
                case QCType.CQC:
                    qCType = QCType.CQC;
                    break;
            }
        }

        public void setupData(Data dataClass)

        {

            if (dataEntry.dataEntryinCode == true)
            {
                pullStaticData();
            }
            else

            {
                // Not worrying about this until I've sorted the static data pull.

                if (lockDate == DateTime.MinValue) { setDefaultsErrorDT(lockDate, "lastLockDate"); }

                if (lockIMF == 0) { lockIMF = setDefaultsErrorInt(lockIMF, "lockIMF"); }
                if (iMFDue == 0) { iMFDue = setDefaultsErrorInt(iMFDue, "iMFDue"); }
                if (dueLaunch == 0) { dueLaunch = setDefaultsErrorInt(dueLaunch, "dueLaunch"); }
            }

            // Might need to use TotalDays here instead. See how it checks out.
            lockIMF = iMFDate.Subtract(lockDate).Days;
            iMFDue = dueDate.Subtract(iMFDate).Days;

            lockME1 = mE1Date.Subtract(lockDate).Days;
            mE1IMF = iMFDate.Subtract(mE1Date).Days;

            iMFME0 = mE0Date.Subtract(iMFDate).Days;
            mE0Due = dueDate.Subtract(mE0Date).Days;

            dueLaunch = 30;

            totalTimeline = Calculator.crunchTimeline(DateTime.Today, lockIMF, iMFDue, lockME1, mE1IMF, iMFME0, mE0Due, dueLaunch);

            // Set Yellow / Red Offsets - need to get a little more specific with these. For now, I'm talking about 4 weeks / 2 weeks / 1 week before launch.
            yellowFactor = 14;
            redFactor = 21;

            Console.WriteLine("Yellow: " + yellowFactor);
            Console.WriteLine("Red: " + redFactor);

            // Send values to crunch
            greenOut = LightSum(0, totalTimeline);
            yellowOut = LightSum(1, totalTimeline);
            redOut = LightSum(2, totalTimeline);
            Program.Output(dueDate, greenOut, yellowOut, redOut);
        }


        // The thing I'm multiplying back with for basic testing at this point. This is not realistic and would require a proportional offset of date / launch.


        DateTime LightSum(int light, int timeline)
        {
            var date = new DateTime();

            switch (light)
            {
                case 0:
                    date = lockDate.AddDays(timeline);
                    break;
                case 1:
                    date = lockDate.AddDays(timeline - yellowFactor);
                    break;
                case 2:
                    date = lockDate.AddDays(timeline - redFactor);
                    break;
                default:
                    break;
            }

            return date;
        }

        public void pullStaticData()
        {
            // Check booleans for casting etc here? This is where I take in the manual inputs which we can assume to be defaults for now.



            lockDate = dataEntry.inputLOCK;
            iMFDate = dataEntry.inputIMF;
            dueDate = dataEntry.inputDue;

            mE0Date = dataEntry.inputME0;
            mE1Date = dataEntry.inputME1;

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
        public static int crunchTimeline(DateTime today, int locktoIMF, int iMFtoDue, int locktoME1, int mE1toIMF, int iMFtoME0, int mE0toDue, int duetoLaunch)
        {
            // This is where I'm gonna put the more actual mathy stuff.

            var timelineSummed = new int();

            switch (Data.mEQCWorkflow)
            {
                case MEQCWorkflow.WF0:
                    timelineSummed = locktoIMF + iMFtoME0 + mE0toDue + duetoLaunch;
                    break;
                case MEQCWorkflow.WF1:
                    timelineSummed = locktoME1 + mE1toIMF + iMFtoDue + duetoLaunch;
                    break;
                case MEQCWorkflow.NoWorkflow:
                    timelineSummed = locktoIMF + iMFtoDue + duetoLaunch;
                    break;
                default:
                    break;
            }
            return timelineSummed;
        }
    }
}