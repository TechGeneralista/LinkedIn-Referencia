#include "ai.h"


extern ADC_HandleTypeDef hadc1;
ADC_ChannelConfTypeDef sConfig = { 0 };


void AIInitialize()
{
    sConfig.Rank = ADC_REGULAR_RANK_1;
    sConfig.SamplingTime = ADC_SAMPLETIME_1CYCLE_5;
    HAL_ADCEx_Calibration_Start(&hadc1);
}

uint16_t ReadADCValue()
{
    HAL_ADC_Start(&hadc1);
    HAL_ADC_PollForConversion(&hadc1, 1000);
    return HAL_ADC_GetValue(&hadc1);
}

uint16_t AI0()
{
    sConfig.Channel = ADC_CHANNEL_1;
    HAL_ADC_ConfigChannel(&hadc1, &sConfig);
    return ReadADCValue();
}

uint16_t AI1()
{
    sConfig.Channel = ADC_CHANNEL_2;
    HAL_ADC_ConfigChannel(&hadc1, &sConfig);
    return ReadADCValue();
}

uint16_t AI2()
{
    sConfig.Channel = ADC_CHANNEL_3;
    HAL_ADC_ConfigChannel(&hadc1, &sConfig);
    return ReadADCValue();
}

uint16_t AI3()
{
    sConfig.Channel = ADC_CHANNEL_4;
    HAL_ADC_ConfigChannel(&hadc1, &sConfig);
    return ReadADCValue();
}
