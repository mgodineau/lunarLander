
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireframeFont
{
    
    private static readonly float[] emptyPath = new float[0];
    
    
    
    private static readonly float[][] font = {
        new float[]{},  // Space
        new float[]{},  // !
        new float[]{},  // "
        new float[]{},  // #
        new float[]{},  // $
        new float[]{},  // %
        new float[]{},  // &
        new float[]{},  // '
        new float[]{0.75f, 0, 0.5f, 0.25f, 0.5f, 0.75f, 0.75f, 1},          // (
        new float[]{0.25f, 0, 0.5f, 0.25f, 0.5f, 0.75f, 0.25f, 1},          // )
        new float[]{},  // *
        new float[]{},  // +
        new float[]{},  // ' ???
        new float[]{0, 0.5f, 1, 0.5f},                                      // -
        new float[]{},  // .
        new float[]{0, 0, 1, 1},                                            // /
        new float[]{0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 1},                    // 0
        new float[]{1, 0, 1, 1},                                            // 1
        new float[]{0, 1, 1, 1, 1, 0.5f, 0, 0.5f, 0, 0, 1, 0},              // 2
        new float[]{0, 0, 1, 0, 1, 0.5f, 0, 0.5f, 1, 0.5f, 1, 1, 0, 1},     // 3
        new float[]{0, 1, 0, 0.5f, 1, 0.5f, 1, 1, 1, 0},                    // 4
        new float[]{0, 0, 1, 0, 1, 0.5f, 0, 0.5f, 0, 1, 1, 1},              // 5
        new float[]{1, 1, 0, 1, 0, 0, 1, 0, 1, 0.5f, 0, 0.5f},              // 6
        new float[]{0, 1, 1, 1, 1, 0},                                      // 7
        new float[]{0, 0.5f, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0.5f, 1, 0.5f},     // 8
        new float[]{0, 0, 1, 0, 1, 1, 0, 1, 0, 0.5f, 1, 0.5f},              // 9
        new float[]{},  // :
        new float[]{},  // ;
        new float[]{1, 0, 0.5f, 0.5f, 1, 1},                                // <
        new float[]{},  // =
        new float[]{0, 0, 0.5f, 0.5f, 0, 1},                                // >
        new float[]{},  // ?
        new float[]{},  // @
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
        new float[]{},  // [
        new float[]{},  // \
        new float[]{},  // ]
        new float[]{},  // ^
        new float[]{},  // _
        new float[]{},  // `
        new float[]{0, 0, 0, 1, 1, 1, 1, 0.5f, 0, 0.5f, 1, 0.5f, 1, 0},                 // a
        new float[]{0, 0.5f, 0, 0, 1, 0, 1, 0.5f, 0, 0.5f, 0, 1, 0.5f, 1, 0.5f, 0.5f},  // b
        new float[]{1, 0, 0, 0, 0, 1, 1, 1},                                            // c
        new float[]{0, 0, 0, 1, 0.5f, 1, 1, 0.5f, 0.5f, 0, 0, 0},                       // d
        new float[]{1, 1, 0, 1, 0, 0.5f, 0.5f, 0.5f, 0, 0.5f, 0, 0, 1, 0},              // e
        new float[]{1, 1, 0, 1, 0, 0.5f, 0.5f, 0.5f, 0, 0.5f, 0, 0},                    // f
        new float[]{0.5f, 0.5f, 1, 0.5f, 0.5f, 0, 0, 0, 0, 1, 1, 1},                    // g
        new float[]{0, 0, 0, 1, 0, 0.5f, 1, 0.5f, 1, 1, 1, 0},                          // h
        new float[]{0, 0, 1, 0, 0.5f, 0, 0.5f, 1, 0, 1, 1, 1},                          // i
        new float[]{0, 0.5f, 0, 0, 0.5f, 0, 0.5f, 1, 0, 1, 1, 1},                       // j
        new float[]{0, 0, 0, 1, 0, 0.5f, 0.5f, 0.5f, 1, 1, 0.5f, 0.5f, 1, 0},           // k
        new float[]{0, 1, 0, 0, 1, 0},                                                  // l
        new float[]{0, 0, 0, 1, 0.5f, 0.5f, 1, 1, 1, 0},                                // m
        new float[]{0, 0, 0, 1, 1, 0, 1, 1},                                            // n
        new float[]{0, 0, 1, 0, 1, 1, 0, 1, 0, 0},                                      // o
        new float[]{0, 0, 0, 1, 1, 1, 1, 0.5f, 0, 0.5f},                                // p
        new float[]{1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0.5f, 0.5f},                          // q
        new float[]{0, 0, 0, 1, 1, 1, 1, 0.5f, 0, 0.5f, 0.5f, 0.5f, 1, 0},              // r
        new float[]{0, 0, 1, 0, 1, 0.5f, 0, 0.5f, 0, 1, 1, 1},                          // s
        new float[]{0, 1, 1, 1, 0.5f, 1, 0.5f, 0},                                      // t
        new float[]{0, 1, 0, 0, 1, 0, 1, 1},                                            // u
        new float[]{0, 1, 0, 0.5f, 0.5f, 0, 1, 0.5f, 1, 1},                             // v
        new float[]{0, 1, 0, 0.5f, 0.5f, 0, 0.5f, 1, 0.5f, 0.5f, 1, 0, 1, 1},           // w
        new float[]{0, 0, 1, 1, 0.5f, 0.5f, 0, 1, 1, 0},                                // x
        new float[]{0.5f, 0, 0.5f, 0.5f, 0, 1, 0.5f, 0.5f, 1, 1},                       // y
        new float[]{1, 0, 0, 0, 1, 1, 0, 1},                                            // z
        new float[]{}, // {
        new float[]{}, // |
        new float[]{}, // }
        new float[]{}, // ~
        new float[]{}, // DEL
    };
    
    public static float[] getCharRawPath( char c ) {
        return font[c-' '];
    }
    
    
}
