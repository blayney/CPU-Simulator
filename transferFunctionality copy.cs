﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.IO;


// Blueprint for external script
public class externalScript : MonoBehaviour
{

    private string ScriptName;
    private string ScriptAuthor;
    private string Description;
    private int noLines;

    // GETTERS
    public string getScriptName()
    {
        return ScriptName;
    }

    public string getScriptAuthor()
    {
        return ScriptAuthor;
    }

    public string getDescription()
    {
        return Description;
    }

    public int getnoLines()
    {
        return noLines;
    }

    // SETTERS
    public void setScriptName(string newName)
    {
        ScriptName = newName;
    }
    public void setScriptAuthor(string newAuthor)
    {
        ScriptAuthor = newAuthor;
    }
    public void setDescription(string newDescription)
    {
        Description = newDescription;
    }



}

// Interface to allow external script functionality to be used in calling AND recalling
public interface SaveScriptInterface
{
    void OutputText();
}


// Main engine code
public class transferFunctionality : MonoBehaviour, SaveScriptInterface
{
    // Variable Declarations
    public int maxTexts = 256;
    // Links execute button
    public Button goButton;
    // Declares text boxes for input
    public Text writesheet, commandBox, console, Core0Temp, PDU0Temp, PDU1Temp, FPGATemp, Core0Load, PDU0Draw, PDU1Draw;
    // Input field
    public InputField inject;
    // Environmental values
    public Text CoreVoltage, PowerLimit, TempLimit, CoreClock, MemoryClock, FanSpeed;
    // Environmental control sliders
    public Slider CVSlider, PLSlider, TLSlider, CCSlider, MCSlider, FSSlider;
    // List of easy access registers
    public int R0, R1, R2, R3, R4, R5, R6, R7, R8, R9, R10, R11, R12, R13, R14, R15;
    // List of easy access register text boxes
    public Text R0V, R1V, R2V, R3V, R4V, R5V, R6V, R7V, R8V, R9V, R10V, R11V, R12V, R13V, R14V, R15V;
    // DRAM matrix
    public int[,] DRAMTBL = new int[32, 9];//DRAM TABLE: FIRST 8 BITS IN ROW = DATA, LAST BIT IS A TAKEN FLAG
                                           // List of matrix text boxes:;::
    public Text R0B0, R0B1, R0B2, R0B3, R0B4, R0B5, R0B6, R0B7;
    public Text R1B0, R1B1, R1B2, R1B3, R1B4, R1B5, R1B6, R1B7;
    public Text R2B0, R2B1, R2B2, R2B3, R2B4, R2B5, R2B6, R2B7;
    public Text R3B0, R3B1, R3B2, R3B3, R3B4, R3B5, R3B6, R3B7;
    public Text R4B0, R4B1, R4B2, R4B3, R4B4, R4B5, R4B6, R4B7;
    public Text R5B0, R5B1, R5B2, R5B3, R5B4, R5B5, R5B6, R5B7;
    public Text R6B0, R6B1, R6B2, R6B3, R6B4, R6B5, R6B6, R6B7;
    public Text R7B0, R7B1, R7B2, R7B3, R7B4, R7B5, R7B6, R7B7;
    public Text R8B0, R8B1, R8B2, R8B3, R8B4, R8B5, R8B6, R8B7;
    public Text R9B0, R9B1, R9B2, R9B3, R9B4, R9B5, R9B6, R9B7;

    public Text R10B0, R10B1, R10B2, R10B3, R10B4, R10B5, R10B6, R10B7;
    public Text R11B0, R11B1, R11B2, R11B3, R11B4, R11B5, R11B6, R11B7;
    public Text R12B0, R12B1, R12B2, R12B3, R12B4, R12B5, R12B6, R12B7;

    public Text DRAM0, DRAM1, DRAM2, DRAM3, DRAM4, DRAM5, DRAM6, DRAM7, DRAM8, DRAM9, DRAM10;

    public Text DRAM11, DRAM12, DRAM13, DRAM14, DRAM15;
    // List of L1 Cache register containers
    public Text L10, L11, L12, L13, L14, L15, L16, L17, L18, L19, L110, L111, L112, L113, L114, L115;
    // Register already assigned queue
    public bool[] RegisterAlreadyAssigned = new bool[16]; // stores Register DRAM assigned state at location of Register number
                                                          // Register pointer stack
    public int[] RegisterDRAMPointer = new int[32]; // stores Register number at location of DRAMTable row
                                                    // L1 pointer stack
    public int[] RegisterL1Pointer = new int[13]; // stores register number at location of destination L1
                                                  // L1 queue
    public bool[] RegisterL1AlreadyAssigned = new bool[16]; // stores Register L1 assigned state at location of register
                                                            // Corresponding L1 text boxes queue
    public bool[] L1AlreadyAssigned = new bool[13]; // stores L1 assignment status at location of L1 register
                                                    // stops having to iterate through matrix to find value and just stores last used one to save time
    public int L1LookupCachedValue = 0;
    // Execution queue for multiline mode
    public int[] CurrentQueue = new int[8];
    // Links oscillator system to main set
    public GameObject Oscillators;
    // Links panel handler system to engine code
    public GameObject PanelHanderObj;
    // Sets multiline mode on or off
    public bool lineByLine = true;


	// HUD VALUE DECLARATIONS :::
	
    public Text GHC0, GHC1, GHC2, GHC3, GHC4, GHC5;
    public Text PCC;
    public Text MAR;
    public Text MDRI;
    public Text MDRO;
    public Text CIR0, CIR1;
    public Text FIFOFO;
    public Text Addr1;
    public Text Addr2;
    public Text Operat;
    public Text NBRXT;

    // Dictionary reference
    Dictionary<string, string> opcodeIDs = new Dictionary<string, string>();


    // PC

    public int ProgramCounter = 0;

    // Output gpc document with script in it
    public void OutputText()
    {
        string path = "MyScript.gpc";
        using (StreamWriter writer = File.CreateText(path))
        {
            writer.Write(writesheet.text);
        }
        console.text += "\n \n created new script file and saved contents of ws to it \n";
    }
    // Output gpc document with console dump in it
    public void OutputDump()
    {
        string path = "ConsoleDump.gpc";
        using (StreamWriter writer = File.CreateText(path))
        {
            writer.Write(console.text);
        }
        console.text += "\n \n created new dump file and saved contents of console to it \n";
    }
    // Bubble sort code
    void bubbleSort(int[] values)
    {
        for (int h = 0; h < values.Length - 1; h++)
        {
            for (int i = 0; i < values.Length - 1; i++)
            {
                int nextVal = values[i + 1];
                if (values[i + 1] < values[i])
                {
                    int temp = values[i];
                    values[i] = values[i + 1];
                    values[i + 1] = temp;
                }
            }
        }

        // Outputs values to console
        for (int j = 0; j < values.Length; j++)
        {
            console.text += "\n " + values[j].ToString();
        }
    }

    // Populates matrix
    void Start()
    {
        OpcodeIDGen();
        int numRows = 32;
        int numCols = 9;
        for (int x = 0; x < numRows; ++x)
        {
            for (int y = 0; y < numCols; ++y)
            {
                DRAMTBL[x, y] = 0;
            }
        }
    }

    // Runs every frame to check if a command was launched.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendText(commandBox.text);
        }

    }
    // populates Dictionary
    public void OpcodeIDGen(){
        opcodeIDs.Add("MOV", "0000");
        opcodeIDs.Add("ADDF", "0001");
        opcodeIDs.Add("MUL", "0010");
        opcodeIDs.Add("META", "0011");
    }
    // Fetches from Dictionary
    public void OpcodeIDFetch(string opcode){
        if(opcodeIDs.ContainsKey(opcode)){
            Operat.text = opcodeIDs[opcode];           

        }else{
            console.text += "\n \n could not find the requested opcode \n \n";

        }

    }
// Manages GH values
    void GHCManager(int newVar){
	int[] covValue = convertTo8BB(newVar);
	GHC0.text = convertTo8BB(newVar).ToString();
	GHC1.text = newVar.ToString();
	string concatCovValue = "";
	for(int ib = 0; ib < 8; ib++){
		concatCovValue += covValue[ib].ToString();
	}
	GHC2.text = concatCovValue;
    }
    void ProgramCounterHandler(){
	// PCC.text = ProgramCounter.ToString();
    	string newPCCValue = "CPU PCC: ";
	//convert to 8BB
	string inStringForm = "";
	int[] arrayForm = convertTo8BB(ProgramCounter);
	for(int ic = 0; ic < 8; ic++){
		inStringForm += arrayForm[ic].ToString();
	}
	newPCCValue += inStringForm + " DENARY: " + ProgramCounter.ToString() + "  FPGA PCC: " + inStringForm;
	PCC.text = newPCCValue;
    }
    // When command is launched, parses command
    private void SendText(string text)

    {
        inject.text = string.Empty;
        console.text += "\n" + text + " ~ sent to command handler";
        inject.Select();
        inject.ActivateInputField();
        PanelHandler ph = PanelHanderObj.GetComponent<PanelHandler>();

        string[] command = text.Split(new char[0]); //splits text into array by spaces

        // sets character that dictates meta command
        char triggerChar = '!';
        // tells handler whether command execution was successful
        bool commandSuccessful = true;
        // Enables or disables success command being displayed because it can be annoying sometimes
        bool displayMsg = true;

        // checks to see if meta command
        if (text[0].Equals(triggerChar))
        {
            switch (command[0])
            {
                case "!exec":
                    //call execution system.
                    string processblock = writesheet.text;
                    // Regex to split execution command by line break, simulating line by line
                    string[] execCommandLbL = Regex.Split(processblock, "\r\n");
                    for (int ia = 0; ia < execCommandLbL.Length; ia++)
                    {
                        SendText(execCommandLbL[ia]);
                        console.text += "Processing " + execCommandLbL[ia];
                    }
                    break;

                // Switches between line by line and multiline mode
                case "!execMode":
                    switch (command[1])
                    {
                        case "block":
                            lineByLine = false;
                            break;
                    }
                    break;
                // Keyboard shortcut to show different panes
                case "!viewPane":
                    switch (command[1])
                    {
                        case "DRAM":
                            ph.ShowDRAMPane();
                            break;
                        case "CPU":
                            ph.ShowSCHEMPane();
                            break;
                        case "GPRAH":
                            ph.ShowGRAPHPane();
                            break;
                    }
                    break;

                // Allows clearing of writesheet or console0
                case "!del":
                    switch (command[1])
                    {
                        case "line":
                            //delete line at command[2]
                            break;
                        case "writesheet":
                            //ask if user is sure

                            writesheet.text = null;
                            break;
                        case "console":
                            console.text = null;
                            displayMsg = false;
                            break;
                    }
                    break;

                // Allows restarts for when index out of bound errors break the command handler
                case "!repost":
                    //reboot vm
                    break;

                // Exports script to gpc file
                case "!export":
                    OutputText();
                    break;

                // Exports console to gpc file
                case "!dump":
                    OutputDump();
                    break;

                // Bubble sorts input values
                case "!sort":
                    console.text += "Please input a list of numbers, separated by commas";
                    string[] intStrings = command[1].Split(',');
                    int[] values = Array.ConvertAll<string, int>(intStrings, int.Parse);
                    for (int iter = 0; iter < values.Length; iter++)
                    {
                        console.text += "\n " + values[iter].ToString() + "\n";
                    }
                    bubbleSort(values);
                    break;

                // Allows fast changing of environment variables
                case "!parameter":
                    switch (command[2])
                    {
                        case "Core0Temp":
                            //edit Core0Temp;
                            changeParameter(command, Core0Temp);
                            break;
                        case "PDU0Temp":
                            //edit PDU0Temp
                            changeParameter(command, PDU0Temp);
                            break;
                        case "PDU1Temp":
                            //edit PDU1Temp
                            changeParameter(command, PDU1Temp);
                            break;
                        case "FPGATemp":
                            //edit FPGATemp;
                            changeParameter(command, FPGATemp);
                            break;
                        case "CoreVoltage":
                            //edit CoreVoltage
                            changeParameter(command, CoreVoltage);
                            CVSlider.value = float.Parse(command[3]);
                            break;
                        case "PowerLimit":
                            //edit PowerLimit;
                            changeParameter(command, PowerLimit);
                            PLSlider.value = float.Parse(command[3]);
                            break;
                        case "TempLimit":
                            //edit TempLimit
                            changeParameter(command, TempLimit);
                            TLSlider.value = float.Parse(command[3]);
                            break;
                        case "CoreClock":
                            //edit CoreClockRate
                            changeParameter(command, CoreClock);
                            CCSlider.value = float.Parse(command[3]);
                            break;
                        case "MemoryClock":
                            //edit MemoryClockRate
                            changeParameter(command, MemoryClock);
                            MCSlider.value = float.Parse(command[3]);
                            break;
                        case "FanSpeed":
                            //edit FanSpeed
                            changeParameter(command, FanSpeed);
                            FSSlider.value = float.Parse(command[3]);
                            break;
                        default:
                            commandSuccessful = false;
                            console.text += "\n" + "command error - can't find parameter";
                            break; // all IDXes need non recursive cases for performance
                    }
                    break;

                // Default break case if no valid command was input
                default:
                    commandSuccessful = false;
                    console.text += "\n" + "command error - can't find command";
                    break;
            }
        }
        else
        {
            // iterates program counter and updates GUI
	    ProgramCounter++;
	    ProgramCounterHandler();
            // parse text and process it as assembly code.
            switch (command[0])
            {
                case ("MOV"):
                    OpcodeIDFetch("MOV");
                    MOVFunction(command);
                    StartCoroutine(TJunction());
                    break;
                case ("ADDF"):
                    OpcodeIDFetch("ADDF");
                    ADDFFunction(command);
                    StartCoroutine(TJunction());
                    break;
                case ("MUL"):
                    OpcodeIDFetch("MUL");
                    MULFunction(command);
                    StartCoroutine(TJunction());
                    break;
            }
            writesheet.text += "\n" + text;
        }

        if (commandSuccessful && displayMsg)
        {
            console.text += "\n" + text + " ~ completed";
        }
        else if (!displayMsg)
        {

        }
        else
        {
            console.text += "\n" + text + " ~ command rejected";
        }

    }
    // Sums two values
    private void ADDFFunction(string[] command)
    {
        console.text += "\n called addf \n";
        string operandA = command[1];
        string operandB = command[2];

        operandA = operandA.Substring(1);
        operandB = operandB.Substring(1);

        if (command[1].Contains('R'))
        {
            switch (int.Parse(operandA))
            {
                case 0:
                    operandA = R0V.text;
                    break;
                case 1:
                    operandA = R1V.text;
                    break;
                case 2:
                    operandA = R2V.text;
                    break;
                case 3:
                    operandA = R3V.text;
                    break;
                case 4:
                    operandA = R4V.text;
                    break;
                case 5:
                    operandA = R5V.text;
                    break;
                case 6:
                    operandA = R6V.text;
                    break;
                case 7:
                    operandA = R7V.text;
                    break;
                case 8:
                    operandA = R8V.text;
                    break;
                case 9:
                    operandA = R9V.text;
                    break;
                case 10:
                    operandA = R10V.text;
                    break;
                case 11:
                    operandA = R11V.text;
                    break;
                case 12:
                    operandA = R12V.text;
                    break;
                case 13:
                    operandA = R13V.text;
                    break;
                case 14:
                    operandA = R14V.text;
                    break;
                case 15:
                    operandA = R15V.text;
                    break;
                default:
                    console.text += "\n \n Register number not found! \n \n";
		    break;

            }

        }


        //console.text += "\n \n VARIABLE DUMP: \n \n";
        //console.text += operandA;
        //console.text += operandB;
        //console.text += "\n END \n \n";
	    FIFOFO.text = operandB;
        string inStringForm = "";
        int[] arrayForm = convertTo8BB(int.Parse(operandB));
        for (int id = 0; id < 8; id++)
        {
            inStringForm += arrayForm[id].ToString();
        }
        FIFOFO.text += " " + inStringForm;	
        int addfresult = int.Parse(operandA) + int.Parse(operandB);
        //console.text += "\n add "+ operandA.ToString() + " and " + operandB.ToString();
        //console.text += addfresult.ToString();
        findRegisterLE(command[3], addfresult);



    }
    // Multiplies two values
    private void MULFunction(string[] command)
    {
        console.text += "\n called mul \n";
        string operandA = command[1];
        string operandB = command[2];

        operandA = operandA.Substring(1);
        operandB = operandB.Substring(1);

        if (command[1].Contains('R'))
        {
            switch (int.Parse(operandA))
            {
                case 0:
                    operandA = R0V.text;
                    break;
                case 1:
                    operandA = R1V.text;
                    break;
                case 2:
                    operandA = R2V.text;
                    break;
                case 3:
                    operandA = R3V.text;
                    break;
                case 4:
                    operandA = R4V.text;
                    break;
                case 5:
                    operandA = R5V.text;
                    break;
                case 6:
                    operandA = R6V.text;
                    break;
                case 7:
                    operandA = R7V.text;
                    break;
                case 8:
                    operandA = R8V.text;
                    break;
                case 9:
                    operandA = R9V.text;
                    break;
                case 10:
                    operandA = R10V.text;
                    break;
                case 11:
                    operandA = R11V.text;
                    break;
                case 12:
                    operandA = R12V.text;
                    break;
                case 13:
                    operandA = R13V.text;
                    break;
                case 14:
                    operandA = R14V.text;
                    break;
                case 15:
                    operandA = R15V.text;
                    break;
                default:
                    console.text += "\n \n Register number not found! \n \n";
		    break;
            }

        }


        //console.text += "\n \n VARIABLE DUMP: \n \n";
        //console.text += operandA;
        //console.text += operandB;
        //console.text += "\n END \n \n";

        int mulresult = int.Parse(operandA) * int.Parse(operandB);
        //console.text += "\n add "+ operandA.ToString() + " and " + operandB.ToString();
        //console.text += addfresult.ToString();
        findRegisterLE(command[3], mulresult);

    }
    private void findRegisterLE(string RegisterToken, int value)
    {
        console.text += "\n called findRegisterLE \n";
        transferFunctionality tf = gameObject.AddComponent<transferFunctionality>();
        RegisterToken = RegisterToken.Substring(1);//remove R
        console.text += "\n register token: " + RegisterToken;

        if (RegisterToken.Equals("*"))
        {

            tf.assignRegister(1, tf.R0);
            StartCoroutine(tf.LambdaT());

        }
        else //if the register number is less than 10
        {
            switch (int.Parse(RegisterToken))
            {//needs default case
                case 0:
                    console.text += "\n called assignRegister 0 with value " + value.ToString() + "\n \n";
                    assignRegister(0, value);
                    break;
                case 1:
                    assignRegister(1, value);
                    break;
                case 2:
                    assignRegister(2, value);
                    break;
                case 3:
                    assignRegister(3, value);
                    break;
                case 4:
                    assignRegister(4, value);
                    break;
                case 5:
                    assignRegister(5, value);
                    break;
                case 6:
                    assignRegister(6, value);
                    break;
                case 7:
                    assignRegister(7, value);
                    break;
                case 8:
                    assignRegister(8, value);
                    break;
                case 9:
                    assignRegister(9, value);
                    break;
                case 10:
                    assignRegister(10, value);
                    break;
                case 11:
                    assignRegister(11, value);
                    break;
                case 12:
                    assignRegister(12, value);
                    break;
                case 13:
                    assignRegister(13, value);
                    break;
                case 14:
                    assignRegister(14, value);
                    break;
                case 15:
                    assignRegister(15, value);
                    break;
            }
        }
    }

    // Updates GUI
    private void changeParameter(string[] command, Text textItem)
    {
        if (command[1].Equals("edit"))
        {
            textItem.text = command[3];
        }
        if (command[1].Equals("reset"))
        {
        }
    }
    // Assigns register
    public void assignRegister(int RegisterNumber, int RegisterValue)
    { //assign immediate value listed to register.
        bool hasMemAssignment = false;

        switch (RegisterNumber)
        {
            case 0:
                R0 = RegisterValue;
                R0V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 0);
                break;
            case 1:
                R1 = RegisterValue;
                R1V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 1);
                break;
            case 2:
                R2 = RegisterValue;
                R2V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 2);
                break;
            case 3:
                R3 = RegisterValue;
                R3V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 3);
                break;
            case 4:
                R4 = RegisterValue;
                R4V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 4);
                break;
            case 5:
                R5 = RegisterValue;
                R5V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 5);
                break;
            case 6:
                R6 = RegisterValue;
                R6V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 6);
                break;
            case 7:
                R7 = RegisterValue;
                R7V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 7);
                break;
            case 8:
                R8 = RegisterValue;
                R8V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 8);
                break;
            case 9:
                R9 = RegisterValue;
                R9V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 9);
                break;
            case 10:
                R10 = RegisterValue;
                R10V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 10);
                break;
            case 11:
                R11 = RegisterValue;
                R11V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 11);
                break;
            case 12:
                R12 = RegisterValue;
                R12V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 12);
                break;
            case 13:
                R13 = RegisterValue;
                R13V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 13);
                break;
            case 14:
                R14 = RegisterValue;
                R14V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 14);
                break;
            case 15:
                R15 = RegisterValue;
                R15V.text = RegisterValue.ToString();
                assignDRAM(RegisterValue, 15);
                break;
        }
    }
    // Resets the environment
    private void resetEnvironment()
    {
        R0 = 0;
        R1 = 0;
        R2 = 0;
        R3 = 0;
        R4 = 0;
        R5 = 0;
        R6 = 0;
        R7 = 0;
        R8 = 0;
        R9 = 0;
        R10 = 0;
        R11 = 0;
        R12 = 0;
        R13 = 0;
        R14 = 0;
        R15 = 0;
        R0V.text = "0";
        R1V.text = "0";
        R2V.text = "0";
        R3V.text = "0";
        R4V.text = "0";
        R5V.text = "0";
        R6V.text = "0";
        R7V.text = "0";
        R8V.text = "0";
        R9V.text = "0";
        R10V.text = "0";
        R11V.text = "0";
        R12V.text = "0";
        R13V.text = "0";
        R14V.text = "0";
        R15V.text = "0";

        R0B0.text = R0B1.text = R0B2.text = R0B3.text = R0B4.text = R0B5.text = R0B6.text = R0B7.text = "0";
        R1B0.text = R1B1.text = R1B2.text = R1B3.text = R1B4.text = R1B5.text = R1B6.text = R1B7.text = "0";
        R2B0.text = R2B1.text = R2B2.text = R2B3.text = R2B4.text = R2B5.text = R2B6.text = R2B7.text = "0";
        R3B0.text = R3B1.text = R3B2.text = R3B3.text = R3B4.text = R3B5.text = R3B6.text = R3B7.text = "0";
        R4B0.text = R4B1.text = R4B2.text = R4B3.text = R4B4.text = R4B5.text = R4B6.text = R4B7.text = "0";
        R5B0.text = R5B1.text = R5B2.text = R5B3.text = R5B4.text = R5B5.text = R5B6.text = R5B7.text = "0";
        R6B0.text = R6B1.text = R6B2.text = R6B3.text = R6B4.text = R6B5.text = R6B6.text = R6B7.text = "0";
        R7B0.text = R7B1.text = R7B2.text = R7B3.text = R7B4.text = R7B5.text = R7B6.text = R7B7.text = "0";
        R8B0.text = R8B1.text = R8B2.text = R8B3.text = R8B4.text = R8B5.text = R8B6.text = R8B7.text = "0";
        R9B0.text = R9B1.text = R9B2.text = R9B3.text = R9B4.text = R9B5.text = R9B6.text = R9B7.text = "0";
        R10B0.text = R10B1.text = R10B2.text = R10B3.text = R10B4.text = R10B5.text = R10B6.text = R10B7.text = "0";
        R11B0.text = R11B1.text = R11B2.text = R11B3.text = R11B4.text = R11B5.text = R11B6.text = R11B7.text = "0";
        R12B0.text = R12B1.text = R12B2.text = R12B3.text = R12B4.text = R12B5.text = R12B6.text = R12B7.text = "0";

        DRAM0.text = DRAM1.text = DRAM2.text = DRAM3.text = DRAM4.text = DRAM5.text = DRAM6.text = DRAM7.text = "0";
        DRAM8.text = DRAM9.text = DRAM10.text = DRAM11.text = DRAM12.text = DRAM13.text = DRAM14.text = DRAM15.text = "0";
        L10.text = L11.text = L12.text = L13.text = L14.text = L15.text = L16.text = L17.text = L18.text = L19.text = "text";
        L110.text = L111.text = L112.text = L113.text = L114.text = L115.text = "0";
        int acc;
        for (acc = 0; acc < 17; acc++)
        {
            RegisterAlreadyAssigned[acc] = false;
            RegisterL1AlreadyAssigned[acc] = false;

        }
        for (acc = 0; acc < 14; acc++)
        {
            L1AlreadyAssigned[acc] = false;

        }
    }

    // Switches to multiline execution model
    private void execMultiLine(string[] command)
    {

        switch (command[0])
        {
            case "MOV":
                MOVFunction(command);
                break;

        }

    }
    // Moves values to and between registers
    public void MOVFunction(string[] command)
    {
        if (command[1][0].ToString().StartsWith('#'.ToString()))
        { // if immediate addressing is used

            command[1] = command[1].Substring(1);//remove #
	    GHCManager(int.Parse(command[1]));
            if (command[2].Length == 2) //if the register number is less than 10
            {
                switch (int.Parse((command[2][1].ToString())))
                {//needs default case
                    case 0:
                        assignRegister(0, int.Parse(command[1]));
                        break;
                    case 1:
                        assignRegister(1, int.Parse(command[1]));
                        break;
                    case 2:
                        assignRegister(2, int.Parse(command[1]));
                        break;
                    case 3:
                        assignRegister(3, int.Parse(command[1]));
                        break;
                    case 4:
                        assignRegister(4, int.Parse(command[1]));
                        break;
                    case 5:
                        assignRegister(5, int.Parse(command[1]));
                        break;
                    case 6:
                        assignRegister(6, int.Parse(command[1]));
                        break;
                    case 7:
                        assignRegister(7, int.Parse(command[1]));
                        break;
                    case 8:
                        assignRegister(8, int.Parse(command[1]));
                        break;
                    case 9:
                        assignRegister(9, int.Parse(command[1]));
                        break;
                    case 10:
                        assignRegister(10, int.Parse(command[1]));
                        break;
                    case 11:
                        assignRegister(11, int.Parse(command[1]));
                        break;
                    case 12:
                        assignRegister(12, int.Parse(command[1]));
                        break;
                    case 13:
                        assignRegister(13, int.Parse(command[1]));
                        break;
                    case 14:
                        assignRegister(14, int.Parse(command[1]));
                        break;
                    case 15:
                        assignRegister(15, int.Parse(command[1]));
                        break;
                }
            }
            else
            { // if the register number is greater than 10, and therefore has two digits to it's number
                switch (int.Parse((command[2][2].ToString())))
                {
                    case 0:
                        assignRegister(10, int.Parse(command[1]));
                        break;
                    case 1:
                        assignRegister(11, int.Parse(command[1]));
                        break;
                    case 2:
                        assignRegister(12, int.Parse(command[1]));
                        break;
                    case 3:
                        assignRegister(13, int.Parse(command[1]));
                        break;
                    case 4:
                        assignRegister(14, int.Parse(command[1]));
                        break;
                    case 5:
                        assignRegister(15, int.Parse(command[1]));
                        break;
                }
            }
        }
        else
        {
            //define destination register number:
            if (command[2].ToString().Equals("R*"))
            {
                assignRegister(1, R0);
                Core0Temp.text += 14.ToString();
                StartCoroutine(LambdaT());
            }
            else
            {
                string destinationRegisterNoStr = command[2].ToString().Substring(1); //remove r
                int destinationRegisterNo = int.Parse(destinationRegisterNoStr);
                switch (command[1].ToString())
                {
                    case "R0":
                        assignRegister(destinationRegisterNo, R0);
                        break;
                    case "R1":
                        assignRegister(destinationRegisterNo, R1);
                        break;
                    case "R2":
                        assignRegister(destinationRegisterNo, R2);
                        break;
                    case "R3":
                        assignRegister(destinationRegisterNo, R3);
                        break;
                    case "R4":
                        assignRegister(destinationRegisterNo, R4);
                        break;
                    case "R5":
                        assignRegister(destinationRegisterNo, R5);
                        break;
                    case "R6":
                        assignRegister(destinationRegisterNo, R6);
                        break;
                    case "R7":
                        assignRegister(destinationRegisterNo, R7);
                        break;
                    case "R8":
                        assignRegister(destinationRegisterNo, R8);
                        break;
                    case "R9":
                        assignRegister(destinationRegisterNo, R9);
                        break;
                    case "R10":
                        assignRegister(destinationRegisterNo, R10);
                        break;
                    case "R11":
                        assignRegister(destinationRegisterNo, R11);
                        break;
                    case "R12":
                        assignRegister(destinationRegisterNo, R12);
                        break;
                    case "R13":
                        assignRegister(destinationRegisterNo, R13);
                        break;
                    case "R14":
                        assignRegister(destinationRegisterNo, R14);
                        break;
                    case "R15":
                        assignRegister(destinationRegisterNo, R15);
                        break;
                }
            }
        }

    }
    // Handles DRAM table
    private void assignDRAM(int Value, int RegisterNumber)
    { // DODGY CODE
        int[] toStore = new int[7];
        if (RegisterAlreadyAssigned[RegisterNumber])
        {
            for (int aa = 0; aa < 32; aa++)
            {
                if (RegisterDRAMPointer[aa] == RegisterNumber)
                {
                    int goTo = aa;
                    toStore = convertTo8BB(Value);
                    DRAMTBL[goTo, 0] = toStore[0];
                    DRAMTBL[goTo, 1] = toStore[1];
                    DRAMTBL[goTo, 2] = toStore[2];
                    DRAMTBL[goTo, 3] = toStore[3];
                    DRAMTBL[goTo, 4] = toStore[4];
                    DRAMTBL[goTo, 5] = toStore[5];
                    DRAMTBL[goTo, 6] = toStore[6];
                    DRAMTBL[goTo, 7] = toStore[7];
                    assignL1(RegisterNumber, goTo);
                    break;
                }
            }
        }
        else
        { // if register is not already assigned, assigns for first time
            for (int ab = 0; ab < 32; ab++)
            {
                if (DRAMTBL[ab, 8] == 0)
                {
                    toStore = convertTo8BB(Value);
                    DRAMTBL[ab, 0] = toStore[0];
                    DRAMTBL[ab, 1] = toStore[1];
                    DRAMTBL[ab, 2] = toStore[2];
                    DRAMTBL[ab, 3] = toStore[3];
                    DRAMTBL[ab, 4] = toStore[4];
                    DRAMTBL[ab, 5] = toStore[5];
                    DRAMTBL[ab, 6] = toStore[6];
                    DRAMTBL[ab, 7] = toStore[7];
                    DRAMTBL[ab, 8] = 1; //signs this register as taken
                    RegisterDRAMPointer[ab] = RegisterNumber; // puts this Register's location in the lookupTable
                    RegisterAlreadyAssigned[RegisterNumber] = true; //registers this register as already assigned to memory
                    assignL1(RegisterNumber, ab);
                    switch (RegisterNumber)
                    {
                        case 0:
                            DRAM0.text = ab.ToString();
                            break;
                        case 1:
                            DRAM1.text = ab.ToString();
                            break;
                        case 2:
                            DRAM2.text = ab.ToString();
                            break;
                        case 3:
                            DRAM3.text = ab.ToString();
                            break;
                        case 4:
                            DRAM4.text = ab.ToString();
                            break;
                        case 5:
                            DRAM5.text = ab.ToString();
                            break;
                        case 6:
                            DRAM6.text = ab.ToString();
                            break;
                        case 7:
                            DRAM7.text = ab.ToString();
                            break;
                        case 8:
                            DRAM8.text = ab.ToString();
                            break;
                        case 9:
                            DRAM9.text = ab.ToString();
                            break;
                        case 10:
                            DRAM10.text = ab.ToString();
                            break;
                        case 11:
                            DRAM11.text = ab.ToString();
                            break;
                        case 12:
                            DRAM12.text = ab.ToString();
                            break;
                        case 13:
                            DRAM13.text = ab.ToString();
                            break;
                        case 14:
                            DRAM14.text = ab.ToString();
                            break;
                        case 15:
                            DRAM15.text = ab.ToString();
                            break;
                    }
                    break;
                }
            }
        }
    }
    // Assigns L1 cache using 8BB
    private void assignL1(int RegisterNumber, int LookupTableRow)
    {
        int L1Location = 0;
        if (!RegisterL1AlreadyAssigned[RegisterNumber])
        {
            if (L1LookupCachedValue == 0)
            {
                R0B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R0B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R0B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R0B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R0B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R0B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R0B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R0B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 0;

            }
            else if (L1LookupCachedValue == 1)
            {
                R1B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R1B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R1B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R1B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R1B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R1B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R1B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R1B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 1;
            }
            else if (L1LookupCachedValue == 2)
            {
                R2B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R2B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R2B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R2B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R2B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R2B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R2B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R2B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 2;
            }
            else if (L1LookupCachedValue == 3)
            {
                R3B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R3B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R3B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R3B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R3B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R3B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R3B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R3B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 3;
            }
            else if (L1LookupCachedValue == 4)
            {
                R4B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R4B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R4B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R4B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R4B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R4B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R4B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R4B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 4;
            }
            else if (L1LookupCachedValue == 5)
            {
                R5B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R5B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R5B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R5B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R5B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R5B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R5B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R5B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 5;
            }
            else if (L1LookupCachedValue == 6)
            {
                R6B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R6B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R6B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R6B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R6B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R6B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R6B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R6B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 6;
            }
            else if (L1LookupCachedValue == 7)
            {
                R7B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R7B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R7B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R7B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R7B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R7B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R7B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R7B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 7;
            }
            else if (L1LookupCachedValue == 8)
            {
                R8B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R8B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R8B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R8B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R8B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R8B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R8B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R8B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 8;
            }
            else if (L1LookupCachedValue == 9)
            {
                R9B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R9B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R9B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R9B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R9B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R9B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R9B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R9B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 9;
            }
            else if (L1LookupCachedValue == 10)
            {
                R10B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R10B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R10B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R10B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R10B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R10B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R10B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R10B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 10;
            }
            else if (L1LookupCachedValue == 11)
            {
                R11B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R11B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R11B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R11B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R11B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R11B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R11B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R11B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 11;
            }
            else if (L1LookupCachedValue == 12)
            {
                R12B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R12B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R12B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R12B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R12B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R12B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R12B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R12B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 12;
            }

            switch (RegisterNumber)
            {
                case 0:
                    L10.text = L1LookupCachedValue.ToString();
                    break;
                case 1:
                    L11.text = L1LookupCachedValue.ToString();
                    break;
                case 2:
                    L12.text = L1LookupCachedValue.ToString();
                    break;
                case 3:
                    L13.text = L1LookupCachedValue.ToString();
                    break;
                case 4:
                    L14.text = L1LookupCachedValue.ToString();
                    break;
                case 5:
                    L15.text = L1LookupCachedValue.ToString();
                    break;
                case 6:
                    L16.text = L1LookupCachedValue.ToString();
                    break;
                case 7:
                    L17.text = L1LookupCachedValue.ToString();
                    break;
                case 8:
                    L18.text = L1LookupCachedValue.ToString();
                    break;
                case 9:
                    L19.text = L1LookupCachedValue.ToString();
                    break;
                case 10:
                    L110.text = L1LookupCachedValue.ToString();
                    break;
                case 11:
                    L111.text = L1LookupCachedValue.ToString();
                    break;
                case 12:
                    L112.text = L1LookupCachedValue.ToString();
                    break;
                case 13:
                    L113.text = L1LookupCachedValue.ToString();
                    break;
                case 14:
                    L114.text = L1LookupCachedValue.ToString();
                    break;
                case 15:
                    L115.text = L1LookupCachedValue.ToString();
                    break;
            }
            L1LookupCachedValue++;
            RegisterL1AlreadyAssigned[RegisterNumber] = true;
            //needs override for when cached l1 reaches 13
        }
        else
        {//update L1
            if (RegisterL1Pointer[0] == RegisterNumber)
            {
                R0B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R0B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R0B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R0B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R0B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R0B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R0B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R0B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 0;

            }
            else if (RegisterL1Pointer[1] == RegisterNumber)
            {
                R1B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R1B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R1B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R1B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R1B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R1B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R1B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R1B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 1;
            }
            else if (RegisterL1Pointer[2] == RegisterNumber)
            {
                R2B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R2B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R2B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R2B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R2B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R2B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R2B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R2B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 2;
            }
            else if (RegisterL1Pointer[3] == RegisterNumber)
            {
                R3B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R3B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R3B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R3B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R3B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R3B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R3B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R3B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 3;
            }
            else if (RegisterL1Pointer[4] == RegisterNumber)
            {
                R4B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R4B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R4B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R4B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R4B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R4B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R4B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R4B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 4;
            }
            else if (RegisterL1Pointer[5] == RegisterNumber)
            {
                R5B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R5B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R5B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R5B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R5B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R5B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R5B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R5B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 5;
            }
            else if (RegisterL1Pointer[6] == RegisterNumber)
            {
                R6B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R6B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R6B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R6B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R6B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R6B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R6B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R6B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 6;
            }
            else if (RegisterL1Pointer[7] == RegisterNumber)
            {
                R7B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R7B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R7B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R7B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R7B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R7B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R7B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R7B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 7;
            }
            else if (RegisterL1Pointer[8] == RegisterNumber)
            {
                R8B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R8B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R8B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R8B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R8B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R8B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R8B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R8B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 8;
            }
            else if (RegisterL1Pointer[9] == RegisterNumber)
            {
                R9B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R9B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R9B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R9B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R9B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R9B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R9B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R9B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 9;
            }
            else if (RegisterL1Pointer[10] == RegisterNumber)
            {
                R10B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R10B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R10B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R10B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R10B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R10B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R10B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R10B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 10;
            }
            else if (RegisterL1Pointer[11] == RegisterNumber)
            {
                R11B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R11B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R11B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R11B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R11B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R11B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R11B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R11B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 11;
            }
            else if (RegisterL1Pointer[12] == RegisterNumber)
            {
                R12B0.text = DRAMTBL[LookupTableRow, 0].ToString();
                R12B1.text = DRAMTBL[LookupTableRow, 1].ToString();
                R12B2.text = DRAMTBL[LookupTableRow, 2].ToString();
                R12B3.text = DRAMTBL[LookupTableRow, 3].ToString();
                R12B4.text = DRAMTBL[LookupTableRow, 4].ToString();
                R12B5.text = DRAMTBL[LookupTableRow, 5].ToString();
                R12B6.text = DRAMTBL[LookupTableRow, 6].ToString();
                R12B7.text = DRAMTBL[LookupTableRow, 7].ToString();
                RegisterL1Pointer[RegisterNumber] = 12;
            }
        }
    }

    // Converts to binary
    private int[] convertTo8BB(int Value)
    {
        int[] returnValue = { 0, 0, 0, 0, 0, 0, 0, 0 };
        int bit = 0;
        int quotient = 0;
        int ordinal = 7;
        while (Value > 0)
        {
            returnValue[ordinal] = Value % 2;
            quotient = Value / 2;
            ordinal--;
            Value = quotient;
        }
        return returnValue;
    }
    public IEnumerator TJunction(){
        int Temperature = 17;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
        yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
yield return new WaitForSeconds(1);
        Temperature--;
        Core0Temp.text = Temperature.ToString();
        Core0Load.text = (Temperature * 5).ToString();
        PDU0Draw.text = (Temperature / 2).ToString();
        yield return new WaitForSeconds(1);
        Core0Temp.text = Core0Load.text = PDU0Draw.text = "0";
        }
    


 // handles time
    public IEnumerator LambdaT()
    {
        float waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(2, R0);
        
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(3, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(4, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(5, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(6, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(7, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(8, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(9, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(10, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(11, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        assignRegister(12, R0);
        waitTime = 1 / MCSlider.value;
        yield return new WaitForSeconds(waitTime);
        waitTime = 1 / MCSlider.value;
        assignRegister(13, R0);
        yield return new WaitForSeconds(waitTime);
        waitTime = 1 / MCSlider.value;
        assignRegister(14, R0);
        yield return new WaitForSeconds(waitTime);
        waitTime = 1 / MCSlider.value;
        assignRegister(15, R0);
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Waited");
    }

}