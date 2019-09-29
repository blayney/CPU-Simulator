using UnityEngine;
using UnityEngine.UI;

public class transferFunctionality : MonoBehaviour
{

    public int maxTexts = 256;
    public Button goButton;
    public Text writesheet, command, console, Core0Temp, PDU0Temp, PDU1Temp, FPGATemp;
    public Text CoreVoltage, PowerLimit, TempLimit, CoreClock, MemoryClock, FanSpeed;
    public Slider CVSlider, PLSlider, TLSlider, CCSlider, MCSlider, FSSlider;
    public int R0, R1, R2, R3, R4, R5, R6, R7, R8, R9, R10, R11, R12, R13, R14, R15;
    public Text R0V, R1V, R2V, R3V, R4V, R5V, R6V, R7V, R8V, R9V, R10V, R11V, R12V, R13V, R14V, R15V;
    public int[,] DRAMTBL =  new int [32,9];//DRAM TABLE: FIRST 8 BITS IN ROW = DATA, LAST BIT IS A TAKEN FLAG
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
    public Text L10, L11, L12, L13, L14, L15, L16, L17, L18, L19, L110, L111, L112, L113, L114, L115;
    public bool[] RegisterAlreadyAssigned = new bool[16]; // stores Register DRAM assigned state at location of Register number
    public int[] RegisterDRAMPointer = new int[32]; // stores Register number at location of DRAMTable row
    public int[] RegisterL1Pointer = new int[13]; // stores register number at location of destination L1
    public bool[] RegisterL1AlreadyAssigned = new bool[16]; // stores Register L1 assigned state at location of register
    public bool[] L1AlreadyAssigned = new bool[13]; // stores L1 assignment status at location of L1 register
    public int L1LookupCachedValue = 0;
    public int[] CurrentQueue = new int[8];
    public GameObject Oscillators;
    public GameObject PanelHanderObj;

    void Start()
    {
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


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendText(command.text);
            command.text = "";
        }

    }
    private void SendText(string text)

    {
        console.text += "\n" + text + " ~ sent to command handler";
        PanelHandler ph = PanelHanderObj.GetComponent<PanelHandler>();

        string[] command = text.Split(new char[0]); //splits text into array by spaces

        char triggerChar = '!';
        bool commandSuccessful = true;
        bool displayMsg = true;

        if (text[0].Equals(triggerChar))

        {
            switch (command[0])
            {
                case "!exec":
                    //call execution system.
                    console.text += "\n" + writesheet.text;
                    break;
                case "!viewPane":
                    switch (command[1]) {
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
                case "!del":
                    switch (command[1])
                    {
                        case "line":
                            //delete line at command[2]
                            break;
                        case "writesheet":
                            //ask if user is sure
                            //delete entire writesheet
                            writesheet.text = null;
                            break;
                        case "console":
                            console.text = null;
                            displayMsg = false;
                            break;
                    }
                    break;
                case "!repost":
                    //reboot vm
                    break;
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
                            break;

                    }
                    break;
                default:
                    commandSuccessful = false;
                    console.text += "\n" + "command error - can't find command";
                    break;
            }
        }
        else {
            // parse text and process it as assembly code.
            switch (command[0]) {
                case ("MOV"):

                    if (command[1][0].ToString().StartsWith('#'.ToString()))
                    { // if immediate addressing is used

                        command[1] = command[1].Substring(1);//remove #

                        if (command[2].Length == 2) //if the register number is less than 10
                        {
                            switch (int.Parse((command[2][1].ToString())))
                            {
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
                    else {
                        //define destination register number:
                        string destinationRegisterNoStr = command[2].ToString().Substring(1); //remove r
                        int destinationRegisterNo = int.Parse(destinationRegisterNoStr);
                        switch (command[1].ToString()) {
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

                    

                   

                    break;
            }
            writesheet.text += "\n" + text;
        }

        if (commandSuccessful && displayMsg)
        {
            console.text += "\n" + text + " ~ completed";
        }
        else if(!displayMsg){

        }
        else {
            console.text += "\n" + text + " ~ command rejected";
        }
        
    }
    private void changeParameter(string[] command, Text textItem){
        if (command[1].Equals("edit")) {
            textItem.text = command[3];
        }
        if (command[1].Equals("reset")) {
            
        }
        
    }

    private void assignRegister(int RegisterNumber, int RegisterValue) { //assign immediate value listed to register.
        bool hasMemAssignment = false;
        switch (RegisterNumber) {
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

    private void assignDRAM(int Value, int RegisterNumber) { // DODGY CODE
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
        else { // if register is not already assigned, assigns for first time
            for (int ab = 0; ab < 32; ab++) {
                if (DRAMTBL[ab, 8] == 0) {
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

    private void assignL1(int RegisterNumber, int LookupTableRow) {
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

            switch (RegisterNumber) {
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
        else {//update L1
            if (RegisterL1Pointer[0]==RegisterNumber)
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


    private int[] convertTo8BB(int Value) {
        int[] returnValue = { 0,0,0,0,0,0,0,0};
        int bit = 0;
        int quotient = 0;
        int ordinal = 7;
        while (Value > 0) {
            returnValue[ordinal] = Value % 2;
            quotient = Value / 2;
            ordinal--;
            Value = quotient;
        }
        return returnValue;
    }

    }







