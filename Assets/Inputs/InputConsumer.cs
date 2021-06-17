using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputConsumer : MonoBehaviour
{
    
    private static Stack<InputConsumer> consumers = new Stack<InputConsumer>();
    
    
    public bool ProcessInput() {
        return consumers.Count > 0 && consumers.Peek() == this;
    }
    
    public void EnableInputProcessing() {
        if( !ProcessInput() ) {
            consumers.Push(this);
        }
    }
    
    public void DisableInputProcessing() {
        if(ProcessInput()) {
            consumers.Pop();
        }
    }
    
    
}
