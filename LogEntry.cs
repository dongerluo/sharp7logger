using System;
using Sharp7;

enum FaultOrWarningState
{
    OFF = 0,
    ON = 1,
    NA = -1
}
class LogEntry
{
    private DateTime plcDateTime;
    private int logEntryNumber;
    private int SRMNumber;
    private string logEntryType;
    private string logEntryDescription;
    private string functionName;
    private string operationMode;
    private FaultOrWarningState faultOrWarningState;
    private int XPosition_mm;
    private int YPosition_mm;
    private int Z1Position_mm;
    private int Z2Position_mm;
    private int Z3Position_mm;
    private int Z4Position_mm;
    private int XTarget_mm;
    private int YTarget_mm;
    private int Z1Target_mm;
    private int Z2Target_mm;
    private int Z3Target_mm;
    private int Z4Target_mm;
    private int XSpeed_mmps;
    private int YSpeed_mmps;
    private int Z1Speed_mmps;
    private int Z2Speed_mmps;
    private int Z3Speed_mmps;
    private int Z4Speed_mmps;

    public LogEntry() {

    }

    public LogEntry(byte[] buffer, int start)
    {
        Parse(buffer, start);
    }

    public int LogEntryNumber { get { return logEntryNumber; } }

    public void Parse(byte[] buffer, int start)
    {
        plcDateTime = S7.GetDateTimeAt(buffer, start); start = start + 8;
        logEntryNumber = S7.GetDIntAt(buffer, start);  start = start + 4;
        SRMNumber = S7.GetIntAt(buffer, start);        start = start + 2;
        logEntryType = S7.GetStringAt(buffer, start);        start = start + 52;
        logEntryDescription = S7.GetStringAt(buffer, start); start = start + 202;
        functionName  = S7.GetStringAt(buffer, start);       start = start + 52;
        operationMode = S7.GetStringAt(buffer, start);       start = start + 52; 
        faultOrWarningState = (FaultOrWarningState)S7.GetIntAt(buffer, start);    start = start + 2;
        XPosition_mm = S7.GetDIntAt(buffer, start); start = start + 4;
        YPosition_mm = S7.GetDIntAt(buffer, start); start = start + 4;
        Z1Position_mm = S7.GetIntAt(buffer, start); start = start + 2;
        Z2Position_mm = S7.GetIntAt(buffer, start); start = start + 2;
        Z3Position_mm = S7.GetIntAt(buffer, start); start = start + 2;
        Z4Position_mm = S7.GetIntAt(buffer, start); start = start + 2;
        XTarget_mm = S7.GetDIntAt(buffer, start); start = start + 4;
        YTarget_mm = S7.GetDIntAt(buffer, start); start = start + 4;
        Z1Target_mm = S7.GetIntAt(buffer, start); start = start + 2;
        Z2Target_mm = S7.GetIntAt(buffer, start); start = start + 2;
        Z3Target_mm = S7.GetIntAt(buffer, start); start = start + 2;
        Z4Target_mm = S7.GetIntAt(buffer, start); start = start + 2;
        XSpeed_mmps = S7.GetIntAt(buffer, start); start = start + 2;
        YSpeed_mmps = S7.GetIntAt(buffer, start); start = start + 2;
        Z1Speed_mmps = S7.GetIntAt(buffer, start); start = start + 2;
        Z2Speed_mmps = S7.GetIntAt(buffer, start); start = start + 2;
        Z3Speed_mmps = S7.GetIntAt(buffer, start); start = start + 2;
        Z4Speed_mmps = S7.GetIntAt(buffer, start); start = start + 2;
    }

    public override string ToString()
    {
        string result;

        result = plcDateTime.ToString("yyyy/MM/dd HH:mm:ss:fff") 
                 + "\t" + logEntryNumber.ToString() 
                 + "\t" + SRMNumber.ToString() + "\t" + logEntryType.ToString() 
                 + "\t" + logEntryDescription.ToString() + "\t" + functionName.ToString() 
                 + "\t" + operationMode.ToString() + "\t" + faultOrWarningState.ToString() 
                 + "\t" + XPosition_mm.ToString() + "\t" + XTarget_mm.ToString() + "\t" + XSpeed_mmps.ToString() 
                 + "\t" + YPosition_mm.ToString() + "\t" + YTarget_mm.ToString() + "\t" + YSpeed_mmps.ToString() 
                 + "\t" + Z1Position_mm.ToString() + "\t" + Z1Target_mm.ToString() + "\t" + Z1Speed_mmps.ToString() 
                 + "\t" + Z2Position_mm.ToString() + "\t" + Z2Target_mm.ToString() + "\t" + Z2Speed_mmps.ToString() 
                 + "\t" + Z3Position_mm.ToString() + "\t" + Z3Target_mm.ToString() + "\t" + Z3Speed_mmps.ToString() 
                 + "\t" + Z4Position_mm.ToString() + "\t" + Z4Target_mm.ToString() + "\t" + Z4Speed_mmps.ToString();

        return result;
    }
}