/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.h
  * @brief          : Header for main.c file.
  *                   This file contains the common defines of the application.
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2020 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under BSD 3-Clause license,
  * the "License"; You may not use this file except in compliance with the
  * License. You may obtain a copy of the License at:
  *                        opensource.org/licenses/BSD-3-Clause
  *
  ******************************************************************************
  */
/* USER CODE END Header */

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __MAIN_H
#define __MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32f1xx_hal.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Exported types ------------------------------------------------------------*/
/* USER CODE BEGIN ET */

/* USER CODE END ET */

/* Exported constants --------------------------------------------------------*/
/* USER CODE BEGIN EC */

/* USER CODE END EC */

/* Exported macro ------------------------------------------------------------*/
/* USER CODE BEGIN EM */

/* USER CODE END EM */

void HAL_TIM_MspPostInit(TIM_HandleTypeDef *htim);

/* Exported functions prototypes ---------------------------------------------*/
void Error_Handler(void);

/* USER CODE BEGIN EFP */

/* USER CODE END EFP */

/* Private defines -----------------------------------------------------------*/
#define PWM1_Pin GPIO_PIN_0
#define PWM1_GPIO_Port GPIOA
#define AI0_Pin GPIO_PIN_1
#define AI0_GPIO_Port GPIOA
#define AI1_Pin GPIO_PIN_2
#define AI1_GPIO_Port GPIOA
#define AI2_Pin GPIO_PIN_3
#define AI2_GPIO_Port GPIOA
#define AI3_Pin GPIO_PIN_4
#define AI3_GPIO_Port GPIOA
#define I0_Pin GPIO_PIN_5
#define I0_GPIO_Port GPIOA
#define PWM0_Pin GPIO_PIN_6
#define PWM0_GPIO_Port GPIOA
#define I1_Pin GPIO_PIN_7
#define I1_GPIO_Port GPIOA
#define I2_Pin GPIO_PIN_0
#define I2_GPIO_Port GPIOB
#define I3_Pin GPIO_PIN_1
#define I3_GPIO_Port GPIOB
#define Status_Pin GPIO_PIN_2
#define Status_GPIO_Port GPIOB
#define I4_Pin GPIO_PIN_10
#define I4_GPIO_Port GPIOB
#define I5_Pin GPIO_PIN_11
#define I5_GPIO_Port GPIOB
#define I6_Pin GPIO_PIN_12
#define I6_GPIO_Port GPIOB
#define I7_Pin GPIO_PIN_13
#define I7_GPIO_Port GPIOB
#define Q7_Pin GPIO_PIN_14
#define Q7_GPIO_Port GPIOB
#define Q6_Pin GPIO_PIN_15
#define Q6_GPIO_Port GPIOB
#define PWM3_Pin GPIO_PIN_8
#define PWM3_GPIO_Port GPIOA
#define Q5_Pin GPIO_PIN_9
#define Q5_GPIO_Port GPIOA
#define Q4_Pin GPIO_PIN_10
#define Q4_GPIO_Port GPIOA
#define Q3_Pin GPIO_PIN_11
#define Q3_GPIO_Port GPIOA
#define Q2_Pin GPIO_PIN_12
#define Q2_GPIO_Port GPIOA
#define Q1_Pin GPIO_PIN_5
#define Q1_GPIO_Port GPIOB
#define PWM2_Pin GPIO_PIN_6
#define PWM2_GPIO_Port GPIOB
#define Q0_Pin GPIO_PIN_7
#define Q0_GPIO_Port GPIOB
/* USER CODE BEGIN Private defines */

/* USER CODE END Private defines */

#ifdef __cplusplus
}
#endif

#endif /* __MAIN_H */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
