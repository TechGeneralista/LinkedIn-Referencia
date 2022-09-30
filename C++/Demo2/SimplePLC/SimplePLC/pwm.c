#include "pwm.h"


extern TIM_HandleTypeDef htim1;
extern TIM_HandleTypeDef htim2;
extern TIM_HandleTypeDef htim3;
extern TIM_HandleTypeDef htim4;


uint32_t TIMclock = 72000000;


void PWMInitialize()
{
    PWM0_SetFreq(1000);
    PWM0_SetDuty(0);
    HAL_TIM_PWM_Start(&htim3, TIM_CHANNEL_1);
    
    PWM1_SetFreq(1000);
    PWM1_SetDuty(0);
    HAL_TIM_PWM_Start(&htim2, TIM_CHANNEL_1);
    
    PWM2_SetFreq(1000);
    PWM2_SetDuty(0);
    HAL_TIM_PWM_Start(&htim4, TIM_CHANNEL_1);
    
    PWM3_SetFreq(1000);
    PWM3_SetDuty(0);
    HAL_TIM_PWM_Start(&htim1, TIM_CHANNEL_1);
}

void SetFreq(TIM_TypeDef *tim, uint32_t freq)
{
    uint32_t ARR = 0;
    uint32_t PRS = 0;
    
    while (1)
    {
        ARR = ((((uint32_t)TIMclock / (PRS + (uint32_t)1)) / freq)) - (uint32_t)1;
		
        if (ARR <= (uint32_t)65535)
        {
            tim->PSC = PRS;
            tim->ARR = ARR;
            break;
        }
        else
        {
            PRS++;
			
            if (PRS == (uint32_t)65535)
            {
                return;
            }
        }
    }
}

void SetDuty(TIM_TypeDef *tim, uint8_t duty)
{
    tim->CCR1 = (uint16_t)((float)tim->ARR * ((float)duty / (float)100));
}

void PWM0_SetFreq(uint32_t freq)
{
    SetFreq(TIM3, freq);
}

void PWM0_SetDuty(uint8_t duty)
{
    SetDuty(TIM3, duty);
}

void PWM1_SetFreq(uint32_t freq)
{
    SetFreq(TIM2, freq);
}

void PWM1_SetDuty(uint8_t duty)
{
    SetDuty(TIM2, duty);
}

void PWM2_SetFreq(uint32_t freq)
{
    SetFreq(TIM4, freq);
}

void PWM2_SetDuty(uint8_t duty)
{
    SetDuty(TIM4, duty);
}

void PWM3_SetFreq(uint32_t freq)
{
    SetFreq(TIM1, freq);
}

void PWM3_SetDuty(uint8_t duty)
{
    SetDuty(TIM1, duty);
}
