using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputConsumer : MonoBehaviour
{
    
    private static Stack<InputConsumer> consumers = new Stack<InputConsumer>();
    
    
    public bool CanProcessInput() {
        return consumers.Count > 0 && consumers.Peek() == this;
    }
    
    public void EnableInputProcessing() {
        if( !CanProcessInput() ) {
            consumers.Push(this);
        }
    }
    
    public void DisableInputProcessing() {
        if(CanProcessInput()) {
            consumers.Pop();
        }
    }
    
    
}
