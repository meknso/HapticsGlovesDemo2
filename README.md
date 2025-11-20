# HapticsGlovesDemo2
Este proyecto quiere simular el potencial háptico que tiene los motores ERM o LRA para generar mejor inmersión al usuario en la realidad mixta

# Haptic Unity → MATLAB → Arduino Pipeline

Este proyecto integra **Unity**, **MATLAB** y **Arduino** para generar **vibraciones hápticas** a partir del movimiento del usuario en el eje **X** dentro de Unity.  
Cada variación de `0.02` en la posición X se envía por **UDP** hacia MATLAB, donde se transforma en un valor PWM enviado al Arduino a través del puerto serial.

---

## 1. Requisitos

- Unity 2021+
- MATLAB con Instrument Control Toolbox
- Arduino Uno / Nano
- Un solo motor háptico conectado al pin PWM **D5**

---

## 2. Flujo del sistema

1. Unity corre la **Escena 0 (5)** y envía coordenadas X por UDP.  
2. MATLAB recibe la posición, la procesa y genera un valor PWM.  
3. MATLAB envía por Serial ese PWM al Arduino.  
4. Arduino genera la vibración en el motor conectado a **D5**.

---

## 3. Ejecución

### Paso 1 — Unity

1. Abrir el proyecto.  
2. Ejecutar **Scene 0 (5)**.  
3. Mover el objeto en X para generar valores enviados por UDP.

---

### Paso 2 — MATLAB

Ejecutar este script antes de mover Unity:

```matlab
u = udpport("datagram","LocalPort",25000);
a = serialport("COM4",115200);
s = 0;
while true
    d = read(u,1,"uint8");
    v = double(d.Data);
    if abs(v - s) >= 0.02
        s = v;
        p = uint8(min(max((v + 1) * 127.5,0),255));
        write(a,p,"uint8");
    end
end

---

### Paso 3 — Arduino

int pwmPin = 5;

void setup() {
  Serial.begin(115200);
  pinMode(pwmPin, OUTPUT);
  analogWrite(pwmPin, 0);
}

void loop() {
  if (Serial.available() > 0) {
    int pwmValue = Serial.read();
    analogWrite(pwmPin, pwmValue);
  }
}

## 4. Conexión del motor háptico

- Se usa un solo motor háptico, por lo cual únicamente se emplea el pin PWM **D5** del Arduino.
- Conexión recomendada:
  - **D5 → entrada PWM del driver del motor**
  - **5V → alimentación del driver**
  - **GND → tierra compartida entre Arduino y driver**

El pin **D5** recibe valores PWM entre **0–255** enviados por MATLAB.  
Valores más altos producen vibración más fuerte.

---

## 5. Estructura general del sistema

Unity → MATLAB → Arduino → Motor Háptico
| | |
| | └── PWM por pin D5
| └── Procesa coordenadas X y genera PWM
└── Envía coordenada X por UDP cada 0.02 unidades
