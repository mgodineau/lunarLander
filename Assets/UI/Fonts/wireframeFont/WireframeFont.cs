
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireframeFont
{
    private const int offsetMaj = 'A';
    private const int offsetMin = 'a';
    
    
    private static readonly float[] emptyPath = new float[0];
    
    private static readonly float[][] font = {
        new float[]{0, 0, 0, 1, 1, 1, 1, 0.5f, 0, 0.5f, 1, 0.5f, 1, 0},                 // A
        new float[]{0, 0.5f, 0, 0, 1, 0, 1, 0.5f, 0, 0.5f, 0, 1, 0.5f, 1, 0.5f, 0.5f},  // B
        new float[]{1, 0, 0, 0, 0, 1, 1, 1},                                            // C
        new float[]{0, 0, 0, 1, 0.5f, 1, 1, 0.5f, 0.5f, 0, 0, 0},                       // D
        new float[]{1, 1, 0, 1, 0, 0.5f, 0.5f, 0.5f, 0, 0.5f, 0, 0, 1, 0},              // E
        new float[]{1, 1, 0, 1, 0, 0.5f, 0.5f, 0.5f, 0, 0.5f, 0, 0},                    // F
        new float[]{0.5f, 0.5f, 1, 0.5f, 0.5f, 0, 0, 0, 0, 1, 1, 1},                    // G
        new float[]{0, 0, 0, 1, 0, 0.5f, 1, 0.5f, 1, 1, 1, 0},                          // H
        new float[]{0, 0, 1, 0, 0.5f, 0, 0.5f, 1, 0, 1, 1, 1},                          // I
        new float[]{0, 0.5f, 0, 0, 0.5f, 0, 0.5f, 1, 0, 1, 1, 1},                       // J
        new float[]{0, 0, 0, 1, 0, 0.5f, 0.5f, 0.5f, 1, 1, 0.5f, 0.5f, 1, 0},           // K
        new float[]{0, 1, 0, 0, 1, 0},                                                  // L
        new float[]{0, 0, 0, 1, 0.5f, 0.5f, 1, 1, 1, 0},                                // M
        new float[]{0, 0, 0, 1, 1, 0, 1, 1},                                            // N
        new float[]{0, 0, 1, 0, 1, 1, 0, 1, 0, 0},                                      // O
        new float[]{0, 0, 0, 1, 1, 1, 1, 0.5f, 0, 0.5f},                                // P
        new float[]{1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0.5f, 0.5f},                          // Q
        new float[]{0, 0, 0, 1, 1, 1, 1, 0.5f, 0, 0.5f, 0.5f, 0.5f, 1, 0},              // R
        new float[]{0, 0, 1, 0, 1, 0.5f, 0, 0.5f, 0, 1, 1, 1},                          // S
        new float[]{0, 1, 1, 1, 0.5f, 1, 0.5f, 0},                                      // T
        new float[]{0, 1, 0, 0, 1, 0, 1, 1},                                            // U
        new float[]{0, 1, 0, 0.5f, 0.5f, 0, 1, 0.5f, 1, 1},                             // V
        new float[]{0, 1, 0, 0.5f, 0.5f, 0, 0.5f, 1, 0.5f, 0.5f, 1, 0, 1, 1},           // W
        new float[]{0, 0, 1, 1, 0.5f, 0.5f, 0, 1, 1, 0},                                // X
        new float[]{0.5f, 0, 0.5f, 0.5f, 0, 1, 0.5f, 0.5f, 1, 1},                       // Y
        new float[]{1, 0, 0, 0, 1, 1, 0, 1},                                            // Z
    };
    
    private static readonly float[] inf = new float[]{1, 0, 0.5f, 0.5f, 1, 1};          // <
    private static readonly float[] sup = new float[]{0, 0, 0.5f, 0.5f, 0, 1};          // >
    
    public static float[] getCharRawPath( char c ) {
        
        int index = c;
        if( index >= offsetMin && index < offsetMin + ('z'-'a') ) {
            index -= offsetMin;
        } else if( index >= offsetMaj && index < offsetMaj + ('z'-'a') ) {
            index -= offsetMaj;
        } else {
            if( c == '<' ) {
                return inf;
            } else if( c == '>' ) {
                return sup;
            }
            return emptyPath;
        }
        
        
        return font[index];
    }
    
    
}
