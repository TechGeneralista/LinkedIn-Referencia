#include "i.h"


uint8_t I0()
{
    if (HAL_GPIO_ReadPin(I0_GPIO_Port, I0_Pin) == GPIO_PIN_SET)
        return 1;
    
    return 0;
}

uint8_t I1()
{
    if (HAL_GPIO_ReadPin(I1_GPIO_Port, I1_Pin) == GPIO_PIN_SET)
        return 1;
    
    return 0;
}

uint8_t I2()
{
    if (HAL_GPIO_ReadPin(I2_GPIO_Port, I2_Pin) == GPIO_PIN_SET)
        return 1;
    
    return 0;
}

uint8_t I3()
{
    if (HAL_GPIO_ReadPin(I3_GPIO_Port, I3_Pin) == GPIO_PIN_SET)
        return 1;
    
    return 0;
}

uint8_t I4()
{
    if (HAL_GPIO_ReadPin(I4_GPIO_Port, I4_Pin) == GPIO_PIN_SET)
        return 1;
    
    return 0;
}

uint8_t I5()
{
    if (HAL_GPIO_ReadPin(I5_GPIO_Port, I5_Pin) == GPIO_PIN_SET)
        return 1;
    
    return 0;
}

uint8_t I6()
{
    if (HAL_GPIO_ReadPin(I6_GPIO_Port, I6_Pin) == GPIO_PIN_SET)
        return 1;
    
    return 0;
}

uint8_t I7()
{
    if (HAL_GPIO_ReadPin(I7_GPIO_Port, I7_Pin) == GPIO_PIN_SET)
        return 1;
    
    return 0;
}
