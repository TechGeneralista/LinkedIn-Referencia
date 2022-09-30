#include "SimplePLCv1.h"

void SimplePLCv1_Initialize()
{
    AIInitialize();
    
    Q0L();
    Q1L();
    Q2L();
    Q3L();
    Q4L();
    Q5L();
    Q6L();
    Q7L();
    
    PWMInitialize();
}
