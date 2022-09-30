#include "program.h"

//GND -> fekete
//CLK -> feh�r
//DIO -> z�ld

void InitCycle()
{
    PWM1_SetFreq(1000);
    PWM1_SetDuty(0);
    
    //Z�ld villog� l�mpa
    PWM0_SetFreq(4);
    PWM0_SetDuty(50);
    
    HAL_Delay(2000);
    
    PWM0_SetFreq(1);
    PWM0_SetDuty(0);
}


uint8_t status = 0;
uint8_t bemelegedesSzamlalo = 0;
uint8_t bemelegedesiIdo = 10;
uint8_t bemelegedesTovabbleptetesSzamlalo = 0;
uint8_t bemelegedesTovabbleptetesIdo = 5;


void SetSpeed()
{
    PWM1_SetFreq(100 + (AI0() / 3));
}


void MainCycle()
{    
    if (status == 0) //bemeleged�s
        {
            Q0H();
            HAL_Delay(1000);
            bemelegedesSzamlalo += 1;
        
            if (I0())
                bemelegedesTovabbleptetesSzamlalo += 1;
            else
                bemelegedesTovabbleptetesSzamlalo = 0;
        
            if (bemelegedesSzamlalo == 90 || bemelegedesTovabbleptetesSzamlalo == bemelegedesTovabbleptetesIdo)
            {
                Q0L();
                PWM0_SetDuty(50);
                while (I0()){HAL_Delay(100);}
                status = 1;
            }
        }
    
    if (status == 1) //motor ind�t�s jobbra
        {
            if (I0())
            {
                PWM0_SetDuty(100);
                while (I0()){HAL_Delay(100); }
                Q1L();
                SetSpeed();
                PWM1_SetDuty(50);
                status = 2;
            }
        }
        
    if (status == 2) //v�rakoz�s a jobb oldali szenzor el�r�s�re
        {
            if (I1() || I0())
            {
                PWM1_SetDuty(0);
                PWM0_SetDuty(50);
                while (I0()){HAL_Delay(100); }
                status = 3;
            }
        }
    
    if (status == 3) //motor ind�t�s balra
        {
            if (I0())
            {
                PWM0_SetDuty(100);
                while (I0()){HAL_Delay(100); }
                Q1H();
                SetSpeed();
                PWM1_SetDuty(50);
                status = 4;
            }
        }
    
    if (status == 4) //v�rakoz�s a bal oldali szenzor el�r�s�re
        {
            if (I2() || I0())
            {
                PWM1_SetDuty(0);
                PWM0_SetDuty(50);
                while (I0()){HAL_Delay(100); }
                status = 1;
            }
        }
}
