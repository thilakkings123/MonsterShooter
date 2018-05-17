using UnityEngine;

public class InstructionBoard : MonoBehaviour {

    [SerializeField] [TextArea(1,4)]
    private string instructions;                            //text to be showned

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))                     //if player is colliding
        {
            GameUI.instance.ShowInstructions(instructions); //show the instructions
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))                     //if player exit
        {
            GameUI.instance.HideInstructions();             //hide the instructions
        }
    }
}
