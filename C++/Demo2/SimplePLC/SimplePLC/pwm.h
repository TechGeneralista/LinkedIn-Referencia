#ifndef PWM_H
#define PWM_H


#include "stm32f1xx_hal.h"


void PWMInitialize();
void PWM0_SetFreq(uint32_t freq);
void PWM0_SetDuty(uint8_t duty);
void PWM1_SetFreq(uint32_t freq);
void PWM1_SetDuty(uint8_t duty);
void PWM2_SetFreq(uint32_t freq);
void PWM2_SetDuty(uint8_t duty);
void PWM3_SetFreq(uint32_t freq);
void PWM3_SetDuty(uint8_t duty);

#endif // !PWM_H

