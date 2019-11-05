using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyHandler : MonoBehaviour
{
    public transferFunctionality tf = new transferFunctionality();
    public InputField inject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void parseCommand(string[] command){
            switch (command[0]){
                case ("MOV"):
                    tf.MOVFunction(command);
                    break;
                case ("ADDF"):
                    ADDFFunction(command);
                    break;
            }
    }
    private void ADDFFunction(string[] command){
        int addfresult = int.Parse(command[1]) + int.Parse(command[2]);
        findRegister(command[3], addfresult);

    }

    private void findRegister(string RegisterToken, int value){
        transferFunctionality tf = new transferFunctionality();
        RegisterToken = RegisterToken.Substring(1);//remove R
        if(RegisterToken.Equals("*")){
            tf.assignRegister(1, tf.R0);
            StartCoroutine(tf.LambdaT());

        }
        else if (RegisterToken.Length == 2) //if the register number is less than 10
        {
            switch (int.Parse((RegisterToken.ToString())))
            {//needs default case
                case 0:
                    tf.assignRegister(0, value);
                    break;
                case 1:
                    tf.assignRegister(1, value);
                    break;
                case 2:
                    tf.assignRegister(2, value);
                    break;
                case 3:
                    tf.assignRegister(3, value);
                    break;
                case 4:
                    tf.assignRegister(4, value);
                    break;
                case 5:
                    tf.assignRegister(5, value);
                    break;
                case 6:
                    tf.assignRegister(6, value);
                    break;
                case 7:
                    tf.assignRegister(7, value);
                    break;
                case 8:
                    tf.assignRegister(8, value);
                    break;
                case 9:
                    tf.assignRegister(9, value);
                    break;
                case 10:
                    tf.assignRegister(10, value);
                    break;
                case 11:
                    tf.assignRegister(11, value);
                    break;
                case 12:
                    tf.assignRegister(12, value);
                    break;
                case 13:
                    tf.assignRegister(13, value);
                    break;
                case 14:
                    tf.assignRegister(14, value);
                    break;
                case 15:
                    tf.assignRegister(15, value);
                    break;
            }
        }
    }


}