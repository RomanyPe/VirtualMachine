# Тестирование базовых концепций Mini-C (MiC)

В этом документе собраны тестовые примеры для проверки работы со статическими массивами, указателями и динамической памятью.

---
## Тест: Методы возвращающие значения и не возвращающие, методы с параметрами, цикл while

```cpp
int a;
int b;
int result;

int sum(int x, int y) {
	int s;
	int i;
	s = 0;
	i = 0;
	while (i < x) {
		s = s + y;
		i = i + 1;
	}
	return s;
}

void m(){
	a = 90;
}

int main() {
	a = 3;
	b = 4;
	a = a + 1;
	m();
	result = sum(a, b);
	return result;
}
```
### Результат
```ams
[Mini-C Compiler] 00000000: 000C0113   LDI r0, data64  data = 0x0000000000000003 (3)
00000010: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001000 (4096)
00000020: 000C0113   LDI r0, data64  data = 0x0000000000000004 (4)
00000030: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001008 (4104)
00000040: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001000 (4096)
00000050: 00002210   MOV r1, r0
00000054: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
00000060: 00002220   ADD r1, r0
00000064: 00004110   MOV r0, r1
00000068: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001000 (4096)
00000078: 000C0052   CALL data64  data = 0x00000000000001E0 (480)
00000088: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001000 (4096)
00000098: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001018 (4120)
000000A8: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001008 (4104)
000000B8: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001020 (4128)
000000C8: 000C0052   CALL data64  data = 0x0000000000000100 (256)
000000D8: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001010 (4112)
000000E8: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001010 (4112)
000000F8: 00000001   HALT
000000FC: 00000001   HALT
00000100: 00000450   PUSH r3
00000104: 00000550   PUSH r4
00000108: 00000650   PUSH r5
0000010C: 00000750   PUSH r6
00000110: 00080411   LOAD.S32 r3, [data64]  data = 0x0000000000001018 (4120)
00000120: 00080511   LOAD.S32 r4, [data64]  data = 0x0000000000001020 (4128)
00000130: 000C0313   LDI r2, data64  data = 0x0000000000000020 (32)
00000140: 00007D21   SUB rSP, r2
00000144: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000150: 00002610   MOV r5, r0
00000154: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000160: 00002710   MOV r6, r0
00000164: 0000E110   MOV r0, r6
00000168: 00002210   MOV r1, r0
0000016C: 00008110   MOV r0, r3
00000170: 00002221   SUB r1, r0
00000174: 000C0044   JL data64  data = 0x0000000000000190 (400)
00000180: 000C0040   JMP data64  data = 0x00000000000001A8 (424)
00000190: 0000A110   MOV r0, r4
00000194: 00002620   ADD r5, r0
00000198: 00000722   INC r6
0000019C: 000C0040   JMP data64  data = 0x0000000000000164 (356)
000001A8: 0000C110   MOV r0, r5
000001AC: 000C0040   JMP data64  data = 0x00000000000001B8 (440)
000001B8: 000C0313   LDI r2, data64  data = 0x0000000000000020 (32)
000001C8: 00007D20   ADD rSP, r2
000001CC: 00000751   POP r6
000001D0: 00000651   POP r5
000001D4: 00000551   POP r4
000001D8: 00000451   POP r3
000001DC: 00000053   RET
000001E0: 000C0113   LDI r0, data64  data = 0x000000000000005A (90)
000001F0: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001000 (4096)
00000200: 00000053   RET
00000204: 00000001   HALT

[Mini-C Compiler] Количество строк кода: 55
[Mini-C Compiler] Размер файла программы: 520 байт
[Unknown Name Device]
 === ЗАПУСК ПОТОКА СИМУЛЯЦИИ ===
[Unknown Name Device] Регистр [0]= 0
[Unknown Name Device] Регистр [1]= 360
[Unknown Name Device] Регистр [2]= 0
[Unknown Name Device] Регистр [3]= 32
[Unknown Name Device] Регистр [4]= 0
[Unknown Name Device] Регистр [5]= 0
[Unknown Name Device] Регистр [6]= 0
[Unknown Name Device] Регистр [7]= 0
[Unknown Name Device] Регистр [8]= 0
[Unknown Name Device] Регистр [9]= 0
[Unknown Name Device] Регистр [10]= 0
[Unknown Name Device] Регистр [11]= 0
[Unknown Name Device] Регистр [12]= 0
[Unknown Name Device] Регистр [13]= 0
[Unknown Name Device] Регистр [14]= 0
[Unknown Name Device] Регистр [15]= 0
[Unknown Name Device] Регистр [16]= 0
[Unknown Name Device] Регистр [17]= 0
[Unknown Name Device] Регистр [18]= 0
[Unknown Name Device] Регистр [19]= 0
[Unknown Name Device] Регистр [20]= 0
[Unknown Name Device] Регистр [21]= 0
[Unknown Name Device] Регистр [22]= 0
[Unknown Name Device] Регистр [23]= 0
[Unknown Name Device] Регистр [24]= 0
[Unknown Name Device] Регистр [25]= 0
[Unknown Name Device] Регистр [26]= 0
[Unknown Name Device] Регистр [27]= 0
[Unknown Name Device] Регистр [28]= 0
[Unknown Name Device] Регистр [29]= 1048576
[Unknown Name Device] Регистр [30]= 0
[Unknown Name Device] Регистр [31]= 252
[Unknown Name Device] IP 252
[Unknown Name Device]
 === ЗАВЕРШЕНИЕ (HALT) ===

```
---

---
## Тест: Методы возвращающие значения и не возвращающие, методы с параметрами, цикл while, инициализация переменных при обьявление

```cpp
int a = 3;
int b = 4;
int result = 0;

int sum(int x, int y) {
	int s = 0;
	int i = 0;
	while (i < x) {
		s = s + y;
		i = i + 1;
	}
	return s;
}

void m(){
	a = 90;
}

int main() {
	a = a + 1;
	m();
	result = sum(a, b);
	return result;
}
```
### Результат
```ams
[Mini-C Compiler] 00000000: 000C0113   LDI r0, data64  data = 0x0000000000000003 (3)
00000010: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001000 (4096)
00000020: 000C0113   LDI r0, data64  data = 0x0000000000000004 (4)
00000030: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001008 (4104)
00000040: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000050: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001010 (4112)
00000060: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001000 (4096)
00000070: 00002210   MOV r1, r0
00000074: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
00000080: 00002220   ADD r1, r0
00000084: 00004110   MOV r0, r1
00000088: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001000 (4096)
00000098: 000C0052   CALL data64  data = 0x0000000000000200 (512)
000000A8: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001000 (4096)
000000B8: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001018 (4120)
000000C8: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001008 (4104)
000000D8: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001020 (4128)
000000E8: 000C0052   CALL data64  data = 0x0000000000000120 (288)
000000F8: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001010 (4112)
00000108: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001010 (4112)
00000118: 00000001   HALT
0000011C: 00000001   HALT
00000120: 00000450   PUSH r3
00000124: 00000550   PUSH r4
00000128: 00000650   PUSH r5
0000012C: 00000750   PUSH r6
00000130: 00080411   LOAD.S32 r3, [data64]  data = 0x0000000000001018 (4120)
00000140: 00080511   LOAD.S32 r4, [data64]  data = 0x0000000000001020 (4128)
00000150: 000C0313   LDI r2, data64  data = 0x0000000000000020 (32)
00000160: 00007D21   SUB rSP, r2
00000164: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000170: 00002610   MOV r5, r0
00000174: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000180: 00002710   MOV r6, r0
00000184: 0000E110   MOV r0, r6
00000188: 00002210   MOV r1, r0
0000018C: 00008110   MOV r0, r3
00000190: 00002221   SUB r1, r0
00000194: 000C0044   JL data64  data = 0x00000000000001B0 (432)
000001A0: 000C0040   JMP data64  data = 0x00000000000001C8 (456)
000001B0: 0000A110   MOV r0, r4
000001B4: 00002620   ADD r5, r0
000001B8: 00000722   INC r6
000001BC: 000C0040   JMP data64  data = 0x0000000000000184 (388)
000001C8: 0000C110   MOV r0, r5
000001CC: 000C0040   JMP data64  data = 0x00000000000001D8 (472)
000001D8: 000C0313   LDI r2, data64  data = 0x0000000000000020 (32)
000001E8: 00007D20   ADD rSP, r2
000001EC: 00000751   POP r6
000001F0: 00000651   POP r5
000001F4: 00000551   POP r4
000001F8: 00000451   POP r3
000001FC: 00000053   RET
00000200: 000C0113   LDI r0, data64  data = 0x000000000000005A (90)
00000210: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001000 (4096)
00000220: 00000053   RET
00000224: 00000001   HALT

[Mini-C Compiler] Количество строк кода: 57
[Mini-C Compiler] Размер файла программы: 552 байт
[Unknown Name Device]
 === ЗАПУСК ПОТОКА СИМУЛЯЦИИ ===
[Unknown Name Device] Регистр [0]= 0
[Unknown Name Device] Регистр [1]= 360
[Unknown Name Device] Регистр [2]= 0
[Unknown Name Device] Регистр [3]= 32
[Unknown Name Device] Регистр [4]= 0
[Unknown Name Device] Регистр [5]= 0
[Unknown Name Device] Регистр [6]= 0
[Unknown Name Device] Регистр [7]= 0
[Unknown Name Device] Регистр [8]= 0
[Unknown Name Device] Регистр [9]= 0
[Unknown Name Device] Регистр [10]= 0
[Unknown Name Device] Регистр [11]= 0
[Unknown Name Device] Регистр [12]= 0
[Unknown Name Device] Регистр [13]= 0
[Unknown Name Device] Регистр [14]= 0
[Unknown Name Device] Регистр [15]= 0
[Unknown Name Device] Регистр [16]= 0
[Unknown Name Device] Регистр [17]= 0
[Unknown Name Device] Регистр [18]= 0
[Unknown Name Device] Регистр [19]= 0
[Unknown Name Device] Регистр [20]= 0
[Unknown Name Device] Регистр [21]= 0
[Unknown Name Device] Регистр [22]= 0
[Unknown Name Device] Регистр [23]= 0
[Unknown Name Device] Регистр [24]= 0
[Unknown Name Device] Регистр [25]= 0
[Unknown Name Device] Регистр [26]= 0
[Unknown Name Device] Регистр [27]= 0
[Unknown Name Device] Регистр [28]= 0
[Unknown Name Device] Регистр [29]= 1048576
[Unknown Name Device] Регистр [30]= 0
[Unknown Name Device] Регистр [31]= 284
[Unknown Name Device] IP 284
[Unknown Name Device]
 === ЗАВЕРШЕНИЕ (HALT) ===
```
---
## Тест: Статичные массивы

```cpp
int global_arr[5];

int sum(int a, int b) {
	return a + b;
}

int main() {
	int local_arr[4];
	local_arr[0] = 10;
	local_arr[1] = 20;
	local_arr[2] = local_arr[0] + local_arr[1];
	local_arr[3] = sum(local_arr[0], local_arr[2]);

	global_arr[0] = local_arr[3];
	global_arr[1] = global_arr[0] + global_arr[0];
	
	int x = global_arr[0] + local_arr[2];
	
	for (int i = 0; i < 4; i = i + 1) {
		local_arr[i] = local_arr[i] + 1;
	}
	print_int(local_arr[0]);
	return 0;
}
```
### Результат:
```
[Mini-C Compiler] 00000000: 000C0313   LDI r2, data64  data = 0x0000000000000020 (32)
00000010: 00007D21   SUB rSP, r2
00000014: 000C0113   LDI r0, data64  data = 0x000000000000000A (10)
00000020: 00002310   MOV r2, r0
00000024: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000030: 00002120   ADD r0, r0
00000034: 00002120   ADD r0, r0
00000038: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
00000048: 0003A320   ADD r2, rSP
0000004C: 00002320   ADD r2, r0
00000050: 00086315   STORE_IND.S32 r2, r2
00000054: 000C0113   LDI r0, data64  data = 0x0000000000000014 (20)
00000060: 00002310   MOV r2, r0
00000064: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
00000070: 00002120   ADD r0, r0
00000074: 00002120   ADD r0, r0
00000078: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
00000088: 0003A320   ADD r2, rSP
0000008C: 00002320   ADD r2, r0
00000090: 00086315   STORE_IND.S32 r2, r2
00000094: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
000000A0: 00002120   ADD r0, r0
000000A4: 00002120   ADD r0, r0
000000A8: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
000000B8: 0003A320   ADD r2, rSP
000000BC: 00002320   ADD r2, r0
000000C0: 00086114   LOAD_IND.S32 r0, r2
000000C4: 00002210   MOV r1, r0
000000C8: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
000000D8: 00002120   ADD r0, r0
000000DC: 00002120   ADD r0, r0
000000E0: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
000000F0: 0003A320   ADD r2, rSP
000000F4: 00002320   ADD r2, r0
000000F8: 00086114   LOAD_IND.S32 r0, r2
000000FC: 00002220   ADD r1, r0
00000100: 00004110   MOV r0, r1
00000104: 00002310   MOV r2, r0
00000108: 000C0113   LDI r0, data64  data = 0x0000000000000002 (2)
00000118: 00002120   ADD r0, r0
0000011C: 00002120   ADD r0, r0
00000120: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
00000130: 0003A320   ADD r2, rSP
00000134: 00002320   ADD r2, r0
00000138: 00086315   STORE_IND.S32 r2, r2
0000013C: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000148: 00002120   ADD r0, r0
0000014C: 00002120   ADD r0, r0
00000150: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
00000160: 0003A320   ADD r2, rSP
00000164: 00002320   ADD r2, r0
00000168: 00086114   LOAD_IND.S32 r0, r2
0000016C: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001018 (4120)
00000178: 000C0113   LDI r0, data64  data = 0x0000000000000002 (2)
00000188: 00002120   ADD r0, r0
0000018C: 00002120   ADD r0, r0
00000190: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
000001A0: 0003A320   ADD r2, rSP
000001A4: 00002320   ADD r2, r0
000001A8: 00086114   LOAD_IND.S32 r0, r2
000001AC: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001020 (4128)
000001B8: 000C0052   CALL data64  data = 0x0000000000000480 (1152)
000001C8: 00002310   MOV r2, r0
000001CC: 000C0113   LDI r0, data64  data = 0x0000000000000003 (3)
000001D8: 00002120   ADD r0, r0
000001DC: 00002120   ADD r0, r0
000001E0: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
000001F0: 0003A320   ADD r2, rSP
000001F4: 00002320   ADD r2, r0
000001F8: 00086315   STORE_IND.S32 r2, r2
000001FC: 000C0113   LDI r0, data64  data = 0x0000000000000003 (3)
00000208: 00002120   ADD r0, r0
0000020C: 00002120   ADD r0, r0
00000210: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
00000220: 0003A320   ADD r2, rSP
00000224: 00002320   ADD r2, r0
00000228: 00086114   LOAD_IND.S32 r0, r2
0000022C: 00002310   MOV r2, r0
00000230: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000240: 00002120   ADD r0, r0
00000244: 00002120   ADD r0, r0
00000248: 000C0313   LDI r2, data64  data = 0x0000000000001000 (4096)
00000258: 00002320   ADD r2, r0
0000025C: 00086315   STORE_IND.S32 r2, r2
00000260: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000270: 00002120   ADD r0, r0
00000274: 00002120   ADD r0, r0
00000278: 000C0313   LDI r2, data64  data = 0x0000000000001000 (4096)
00000288: 00002320   ADD r2, r0
0000028C: 00086114   LOAD_IND.S32 r0, r2
00000290: 00002210   MOV r1, r0
00000294: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
000002A0: 00002120   ADD r0, r0
000002A4: 00002120   ADD r0, r0
000002A8: 000C0313   LDI r2, data64  data = 0x0000000000001000 (4096)
000002B8: 00002320   ADD r2, r0
000002BC: 00086114   LOAD_IND.S32 r0, r2
000002C0: 00002220   ADD r1, r0
000002C4: 00004110   MOV r0, r1
000002C8: 00002310   MOV r2, r0
000002CC: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
000002D8: 00002120   ADD r0, r0
000002DC: 00002120   ADD r0, r0
000002E0: 000C0313   LDI r2, data64  data = 0x0000000000001000 (4096)
000002F0: 00002320   ADD r2, r0
000002F4: 00086315   STORE_IND.S32 r2, r2
000002F8: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000308: 00002120   ADD r0, r0
0000030C: 00002120   ADD r0, r0
00000310: 000C0313   LDI r2, data64  data = 0x0000000000001000 (4096)
00000320: 00002320   ADD r2, r0
00000324: 00086114   LOAD_IND.S32 r0, r2
00000328: 00002210   MOV r1, r0
0000032C: 000C0113   LDI r0, data64  data = 0x0000000000000002 (2)
00000338: 00002120   ADD r0, r0
0000033C: 00002120   ADD r0, r0
00000340: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
00000350: 0003A320   ADD r2, rSP
00000354: 00002320   ADD r2, r0
00000358: 00086114   LOAD_IND.S32 r0, r2
0000035C: 00002220   ADD r1, r0
00000360: 00004110   MOV r0, r1
00000364: 00002410   MOV r3, r0
00000368: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000378: 00002510   MOV r4, r0
0000037C: 0000A110   MOV r0, r4
00000380: 00002210   MOV r1, r0
00000384: 000C0113   LDI r0, data64  data = 0x0000000000000004 (4)
00000390: 00002221   SUB r1, r0
00000394: 000C0044   JL data64  data = 0x00000000000003B0 (944)
000003A0: 000C0040   JMP data64  data = 0x0000000000000430 (1072)
000003B0: 0000A110   MOV r0, r4
000003B4: 00002120   ADD r0, r0
000003B8: 00002120   ADD r0, r0
000003BC: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
000003C8: 0003A320   ADD r2, rSP
000003CC: 00002320   ADD r2, r0
000003D0: 00086114   LOAD_IND.S32 r0, r2
000003D4: 00002210   MOV r1, r0
000003D8: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
000003E8: 00002220   ADD r1, r0
000003EC: 00004110   MOV r0, r1
000003F0: 00002310   MOV r2, r0
000003F4: 0000A110   MOV r0, r4
000003F8: 00002120   ADD r0, r0
000003FC: 00002120   ADD r0, r0
00000400: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
00000410: 0003A320   ADD r2, rSP
00000414: 00002320   ADD r2, r0
00000418: 00086315   STORE_IND.S32 r2, r2
0000041C: 00000522   INC r4
00000420: 000C0040   JMP data64  data = 0x000000000000037C (892)
00000430: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000440: 00002120   ADD r0, r0
00000444: 00002120   ADD r0, r0
00000448: 000C0313   LDI r2, data64  data = 0x0000000000000000 (0)
00000458: 0003A320   ADD r2, rSP
0000045C: 00002320   ADD r2, r0
00000460: 00086114   LOAD_IND.S32 r0, r2
00000464: 00000162   UNKNOWN 0x62
00000468: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000478: 00000001   HALT
0000047C: 00000001   HALT
00000480: 00000450   PUSH r3
00000484: 00000550   PUSH r4
00000488: 00080411   LOAD.S32 r3, [data64]  data = 0x0000000000001018 (4120)
00000498: 00080511   LOAD.S32 r4, [data64]  data = 0x0000000000001020 (4128)
000004A8: 000C0313   LDI r2, data64  data = 0x0000000000000010 (16)
000004B8: 00007D21   SUB rSP, r2
000004BC: 00008110   MOV r0, r3
000004C0: 00002210   MOV r1, r0
000004C4: 0000A110   MOV r0, r4
000004C8: 00002220   ADD r1, r0
000004CC: 00004110   MOV r0, r1
000004D0: 000C0040   JMP data64  data = 0x00000000000004E0 (1248)
000004E0: 000C0313   LDI r2, data64  data = 0x0000000000000010 (16)
000004F0: 00007D20   ADD rSP, r2
000004F4: 00000551   POP r4
000004F8: 00000451   POP r3
000004FC: 00000053   RET
00000500: 00000001   HALT

[Mini-C Compiler] Количество строк кода: 181
[Mini-C Compiler] Размер файла программы: 1284 байт
[Unknown Name Device]
 === ЗАПУСК ПОТОКА СИМУЛЯЦИИ ===
[Unknown Name Device] 1048544
[Unknown Name Device] Регистр [0]= 0
[Unknown Name Device] Регистр [1]= 0
[Unknown Name Device] Регистр [2]= 0
[Unknown Name Device] Регистр [3]= 1048544
[Unknown Name Device] Регистр [4]= 1052648
[Unknown Name Device] Регистр [5]= 4
[Unknown Name Device] Регистр [6]= 0
[Unknown Name Device] Регистр [7]= 0
[Unknown Name Device] Регистр [8]= 0
[Unknown Name Device] Регистр [9]= 0
[Unknown Name Device] Регистр [10]= 0
[Unknown Name Device] Регистр [11]= 0
[Unknown Name Device] Регистр [12]= 0
[Unknown Name Device] Регистр [13]= 0
[Unknown Name Device] Регистр [14]= 0
[Unknown Name Device] Регистр [15]= 0
[Unknown Name Device] Регистр [16]= 0
[Unknown Name Device] Регистр [17]= 0
[Unknown Name Device] Регистр [18]= 0
[Unknown Name Device] Регистр [19]= 0
[Unknown Name Device] Регистр [20]= 0
[Unknown Name Device] Регистр [21]= 0
[Unknown Name Device] Регистр [22]= 0
[Unknown Name Device] Регистр [23]= 0
[Unknown Name Device] Регистр [24]= 0
[Unknown Name Device] Регистр [25]= 0
[Unknown Name Device] Регистр [26]= 0
[Unknown Name Device] Регистр [27]= 0
[Unknown Name Device] Регистр [28]= 0
[Unknown Name Device] Регистр [29]= 1048544
[Unknown Name Device] Регистр [30]= 0
[Unknown Name Device] Регистр [31]= 1148
[Unknown Name Device] IP 1148
[Unknown Name Device]
 === ЗАВЕРШЕНИЕ (HALT) ===

```
---

## Тест: Указатели как параметры функций

```cpp
int a = 5;
int b = 10;
int *p;
int *q;

void swap(int *x, int *y) {
	int temp = *x;
	*x = *y;
	*y = temp;
}

int main() {
	p = &a;
	q = &b;
	swap(p, q);
	return *p;
}
```

### Результат
```asm
[Mini-C Compiler] 00000000: 000C0113   LDI r0, data64  data = 0x0000000000000005 (5)
00000010: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001000 (4096)
00000020: 000C0113   LDI r0, data64  data = 0x000000000000000A (10)
00000030: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001008 (4104)
00000040: 000C0113   LDI r0, data64  data = 0x0000000000001000 (4096)
00000050: 000C0112   STORE.S64 [data64], r0  data = 0x0000000000001010 (4112)
00000060: 000C0113   LDI r0, data64  data = 0x0000000000001008 (4104)
00000070: 000C0112   STORE.S64 [data64], r0  data = 0x0000000000001018 (4120)
00000080: 000C0111   LOAD.S64 r0, [data64]  data = 0x0000000000001010 (4112)
00000090: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001020 (4128)
000000A0: 000C0111   LOAD.S64 r0, [data64]  data = 0x0000000000001018 (4120)
000000B0: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001028 (4136)
000000C0: 000C0052   CALL data64  data = 0x00000000000000EC (236)
000000D0: 000C0111   LOAD.S64 r0, [data64]  data = 0x0000000000001010 (4112)
000000E0: 00082114   LOAD_IND.S32 r0, r0
000000E4: 00000001   HALT
000000E8: 00000001   HALT
000000EC: 00000450   PUSH r3
000000F0: 00000550   PUSH r4
000000F4: 00000650   PUSH r5
000000F8: 000C0411   LOAD.S64 r3, [data64]  data = 0x0000000000001020 (4128)
00000108: 000C0511   LOAD.S64 r4, [data64]  data = 0x0000000000001028 (4136)
00000118: 000C0313   LDI r2, data64  data = 0x0000000000000018 (24)
00000128: 00007D21   SUB rSP, r2
0000012C: 00008110   MOV r0, r3
00000130: 00082114   LOAD_IND.S32 r0, r0
00000134: 00002610   MOV r5, r0
00000138: 0000A110   MOV r0, r4
0000013C: 00082114   LOAD_IND.S32 r0, r0
00000140: 00002210   MOV r1, r0
00000144: 00008110   MOV r0, r3
00000148: 00082215   STORE_IND.S32 r1, r0
0000014C: 0000C110   MOV r0, r5
00000150: 00002210   MOV r1, r0
00000154: 0000A110   MOV r0, r4
00000158: 00082215   STORE_IND.S32 r1, r0
0000015C: 000C0313   LDI r2, data64  data = 0x0000000000000018 (24)
00000168: 00007D20   ADD rSP, r2
0000016C: 00000651   POP r5
00000170: 00000551   POP r4
00000174: 00000451   POP r3
00000178: 00000053   RET
0000017C: 00000001   HALT

[Mini-C Compiler] Количество строк кода: 43
[Mini-C Compiler] Размер файла программы: 384 байт
[Unknown Name Device]
 === ЗАПУСК ПОТОКА СИМУЛЯЦИИ ===
[INFO] Программа успешно завершила работу (HALT). Ip [232]

[Unknown Name Device] Регистр [0]= 0
[Unknown Name Device] Регистр [1]= 10
[Unknown Name Device] Регистр [2]= 5
[Unknown Name Device] Регистр [3]= 24
[Unknown Name Device] Регистр [4]= 0
[Unknown Name Device] Регистр [5]= 0
[Unknown Name Device] Регистр [6]= 0
[Unknown Name Device] Регистр [7]= 0
[Unknown Name Device] Регистр [8]= 0
[Unknown Name Device] Регистр [9]= 0
[Unknown Name Device] Регистр [10]= 0
[Unknown Name Device] Регистр [11]= 0
[Unknown Name Device] Регистр [12]= 0
[Unknown Name Device] Регистр [13]= 0
[Unknown Name Device] Регистр [14]= 0
[Unknown Name Device] Регистр [15]= 0
[Unknown Name Device] Регистр [16]= 0
[Unknown Name Device] Регистр [17]= 0
[Unknown Name Device] Регистр [18]= 0
[Unknown Name Device] Регистр [19]= 0
[Unknown Name Device] Регистр [20]= 0
[Unknown Name Device] Регистр [21]= 0
[Unknown Name Device] Регистр [22]= 0
[Unknown Name Device] Регистр [23]= 0
[Unknown Name Device] Регистр [24]= 0
[Unknown Name Device] Регистр [25]= 0
[Unknown Name Device] Регистр [26]= 0
[Unknown Name Device] Регистр [27]= 0
[Unknown Name Device] Регистр [28]= 0
[Unknown Name Device] Регистр [29]= 1048576
[Unknown Name Device] Регистр [30]= 0
[Unknown Name Device] Регистр [31]= 232
[Unknown Name Device] IP 232
[Unknown Name Device]
 === ЗАВЕРШЕНИЕ (HALT) ===

```

---

## Тест: Базовые указатели

```cpp
int a = 5;
int b = 10;
int *p;

int main() {
	p = &a;
	int c = *p;   // c = 5
	*p = 20;      // a = 20
	return a;     // 20
}
```
### Результат:
```asm
[Mini-C Compiler] 00000000: 000C0113   LDI r0, data64  data = 0x0000000000000005 (5)
00000010: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001000 (4096)
00000020: 000C0113   LDI r0, data64  data = 0x000000000000000A (10)
00000030: 00080112   STORE.S32 [data64], r0  data = 0x0000000000001008 (4104)
00000040: 000C0313   LDI r2, data64  data = 0x0000000000000008 (8)
00000050: 00007D21   SUB rSP, r2
00000054: 000C0113   LDI r0, data64  data = 0x0000000000001000 (4096)
00000060: 000C0112   STORE.S64 [data64], r0  data = 0x0000000000001010 (4112)
00000070: 000C0111   LOAD.S64 r0, [data64]  data = 0x0000000000001010 (4112)
00000080: 00082114   LOAD_IND.S32 r0, r0
00000084: 00002410   MOV r3, r0
00000088: 000C0113   LDI r0, data64  data = 0x0000000000000014 (20)
00000098: 00002210   MOV r1, r0
0000009C: 000C0111   LOAD.S64 r0, [data64]  data = 0x0000000000001010 (4112)
000000A8: 00082215   STORE_IND.S32 r1, r0
000000AC: 00080111   LOAD.S32 r0, [data64]  data = 0x0000000000001000 (4096)
000000B8: 00000001   HALT
000000BC: 00000001   HALT
000000C0: 00000001   HALT

[Mini-C Compiler] Количество строк кода: 19
[Mini-C Compiler] Размер файла программы: 196 байт
[Unknown Name Device]
=== ЗАПУСК ПОТОКА СИМУЛЯЦИИ ===
[Unknown Name Device] Регистр [0]= 0
[Unknown Name Device] Регистр [1]= 20
[Unknown Name Device] Регистр [2]= 20
[Unknown Name Device] Регистр [3]= 8
[Unknown Name Device] Регистр [4]= 5
[Unknown Name Device] Регистр [5]= 0
[Unknown Name Device] Регистр [6]= 0
[Unknown Name Device] Регистр [7]= 0
[Unknown Name Device] Регистр [8]= 0
[Unknown Name Device] Регистр [9]= 0
[Unknown Name Device] Регистр [10]= 0
[Unknown Name Device] Регистр [11]= 0
[Unknown Name Device] Регистр [12]= 0
[Unknown Name Device] Регистр [13]= 0
[Unknown Name Device] Регистр [14]= 0
[Unknown Name Device] Регистр [15]= 0
[Unknown Name Device] Регистр [16]= 0
[Unknown Name Device] Регистр [17]= 0
[Unknown Name Device] Регистр [18]= 0
[Unknown Name Device] Регистр [19]= 0
[Unknown Name Device] Регистр [20]= 0
[Unknown Name Device] Регистр [21]= 0
[Unknown Name Device] Регистр [22]= 0
[Unknown Name Device] Регистр [23]= 0
[Unknown Name Device] Регистр [24]= 0
[Unknown Name Device] Регистр [25]= 0
[Unknown Name Device] Регистр [26]= 0
[Unknown Name Device] Регистр [27]= 0
[Unknown Name Device] Регистр [28]= 0
[Unknown Name Device] Регистр [29]= 1048568
[Unknown Name Device] Регистр [30]= 0
[Unknown Name Device] Регистр [31]= 188
[Unknown Name Device] IP 188
[Unknown Name Device]
 === ЗАВЕРШЕНИЕ (HALT) ===

```
---

## Тест: Динамическая память, вывод чисел на экран командой print_int(value);

```cpp
int main(){
	int* arr = new int[10];   // arr = alloc(10 * sizeof(int))
	arr[0] = 5;
	arr[1] = 10;
	print_int(arr[0]);        // 5
	delete arr;               // пока ничего не делает
	return 0;
}
```
### Резульат:
```asm
[Mini-C Compiler] 00000000: 000C0313   LDI r2, data64  data = 0x0000000000000008 (8)
00000010: 00007D21   SUB rSP, r2
00000014: 000C0113   LDI r0, data64  data = 0x000000000000000A (10)
00000020: 00002120   ADD r0, r0
00000024: 00002120   ADD r0, r0
00000028: 00000070   UNKNOWN 0x70
0000002C: 00002410   MOV r3, r0
00000030: 000C0113   LDI r0, data64  data = 0x0000000000000005 (5)
00000040: 00002310   MOV r2, r0
00000044: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000050: 00002210   MOV r1, r0
00000054: 00008110   MOV r0, r3
00000058: 00004220   ADD r1, r1
0000005C: 00004220   ADD r1, r1
00000060: 00004120   ADD r0, r1
00000064: 00082315   STORE_IND.S32 r2, r0
00000068: 000C0113   LDI r0, data64  data = 0x000000000000000A (10)
00000078: 00002310   MOV r2, r0
0000007C: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
00000088: 00002210   MOV r1, r0
0000008C: 00008110   MOV r0, r3
00000090: 00004220   ADD r1, r1
00000094: 00004220   ADD r1, r1
00000098: 00004120   ADD r0, r1
0000009C: 00082315   STORE_IND.S32 r2, r0
000000A0: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
000000B0: 00002210   MOV r1, r0
000000B4: 00008110   MOV r0, r3
000000B8: 00004220   ADD r1, r1
000000BC: 00004220   ADD r1, r1
000000C0: 00004120   ADD r0, r1
000000C4: 00082114   LOAD_IND.S32 r0, r0
000000C8: 00000162   UNKNOWN 0x62
000000CC: 00008110   MOV r0, r3
000000D0: 00000071   UNKNOWN 0x71
000000D4: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
000000E0: 00000001   HALT
000000E4: 00000001   HALT
000000E8: 00000001   HALT

[Mini-C Compiler] Количество строк кода: 39
[Mini-C Compiler] Размер файла программы: 236 байт
[Unknown Name Device]
 === ЗАПУСК ПОТОКА СИМУЛЯЦИИ ===
[Unknown Name Device] 5
[Unknown Name Device] Регистр [0]= 0
[Unknown Name Device] Регистр [1]= 0
[Unknown Name Device] Регистр [2]= 0
[Unknown Name Device] Регистр [3]= 10
[Unknown Name Device] Регистр [4]= 0
[Unknown Name Device] Регистр [5]= 0
[Unknown Name Device] Регистр [6]= 0
[Unknown Name Device] Регистр [7]= 0
[Unknown Name Device] Регистр [8]= 0
[Unknown Name Device] Регистр [9]= 0
[Unknown Name Device] Регистр [10]= 0
[Unknown Name Device] Регистр [11]= 0
[Unknown Name Device] Регистр [12]= 0
[Unknown Name Device] Регистр [13]= 0
[Unknown Name Device] Регистр [14]= 0
[Unknown Name Device] Регистр [15]= 0
[Unknown Name Device] Регистр [16]= 0
[Unknown Name Device] Регистр [17]= 0
[Unknown Name Device] Регистр [18]= 0
[Unknown Name Device] Регистр [19]= 0
[Unknown Name Device] Регистр [20]= 0
[Unknown Name Device] Регистр [21]= 0
[Unknown Name Device] Регистр [22]= 0
[Unknown Name Device] Регистр [23]= 0
[Unknown Name Device] Регистр [24]= 0
[Unknown Name Device] Регистр [25]= 0
[Unknown Name Device] Регистр [26]= 0
[Unknown Name Device] Регистр [27]= 0
[Unknown Name Device] Регистр [28]= 0
[Unknown Name Device] Регистр [29]= 1048568
[Unknown Name Device] Регистр [30]= 40
[Unknown Name Device] Регистр [31]= 228
[Unknown Name Device] IP 228
[Unknown Name Device]
 === ЗАВЕРШЕНИЕ (HALT) ===
```
---

## Тест: Динамическая память, вывод чисел на экран командой print_int(value);, вывод все чисел массива и выход за его границы, "мусорные" данные

```cpp
int main(){
	int* arr = new int[10];   // arr = alloc(10 * sizeof(int))
	arr[0] = 5;
	arr[1] = 10;
	
	print_int(arr[0]);        // 5
	print_int(arr[1]);
	print_int(arr[2]);
	print_int(arr[3]);
	print_int(arr[4]);
	print_int(arr[5]);
	print_int(arr[6]);
	print_int(arr[7]);
	print_int(arr[8]);
	print_int(arr[9]);
	print_int(arr[10]);
	print_int(arr[11]);
	print_int(arr[12]);
	
	delete arr;               // пока ничего не делает
	return 0;
}
```

### Резульат:
```asm
[Mini-C Compiler] 00000000: 000C0313   LDI r2, data64  data = 0x0000000000000008 (8)
00000010: 00007D21   SUB rSP, r2
00000014: 000C0113   LDI r0, data64  data = 0x000000000000000A (10)
00000020: 00002120   ADD r0, r0
00000024: 00002120   ADD r0, r0
00000028: 00000070   ALLOC rZ
0000002C: 00002410   MOV r3, r0
00000030: 000C0113   LDI r0, data64  data = 0x0000000000000005 (5)
00000040: 00002310   MOV r2, r0
00000044: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
00000050: 00002210   MOV r1, r0
00000054: 00008110   MOV r0, r3
00000058: 00004220   ADD r1, r1
0000005C: 00004220   ADD r1, r1
00000060: 00004120   ADD r0, r1
00000064: 00082315   STORE_IND.S32 r2, r0
00000068: 000C0113   LDI r0, data64  data = 0x000000000000000A (10)
00000078: 00002310   MOV r2, r0
0000007C: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
00000088: 00002210   MOV r1, r0
0000008C: 00008110   MOV r0, r3
00000090: 00004220   ADD r1, r1
00000094: 00004220   ADD r1, r1
00000098: 00004120   ADD r0, r1
0000009C: 00082315   STORE_IND.S32 r2, r0
000000A0: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
000000B0: 00002210   MOV r1, r0
000000B4: 00008110   MOV r0, r3
000000B8: 00004220   ADD r1, r1
000000BC: 00004220   ADD r1, r1
000000C0: 00004120   ADD r0, r1
000000C4: 00082114   LOAD_IND.S32 r0, r0
000000C8: 00000162   PRINT_INT r0
000000CC: 000C0113   LDI r0, data64  data = 0x0000000000000001 (1)
000000D8: 00002210   MOV r1, r0
000000DC: 00008110   MOV r0, r3
000000E0: 00004220   ADD r1, r1
000000E4: 00004220   ADD r1, r1
000000E8: 00004120   ADD r0, r1
000000EC: 00082114   LOAD_IND.S32 r0, r0
000000F0: 00000162   PRINT_INT r0
000000F4: 000C0113   LDI r0, data64  data = 0x0000000000000002 (2)
00000100: 00002210   MOV r1, r0
00000104: 00008110   MOV r0, r3
00000108: 00004220   ADD r1, r1
0000010C: 00004220   ADD r1, r1
00000110: 00004120   ADD r0, r1
00000114: 00082114   LOAD_IND.S32 r0, r0
00000118: 00000162   PRINT_INT r0
0000011C: 000C0113   LDI r0, data64  data = 0x0000000000000003 (3)
00000128: 00002210   MOV r1, r0
0000012C: 00008110   MOV r0, r3
00000130: 00004220   ADD r1, r1
00000134: 00004220   ADD r1, r1
00000138: 00004120   ADD r0, r1
0000013C: 00082114   LOAD_IND.S32 r0, r0
00000140: 00000162   PRINT_INT r0
00000144: 000C0113   LDI r0, data64  data = 0x0000000000000004 (4)
00000150: 00002210   MOV r1, r0
00000154: 00008110   MOV r0, r3
00000158: 00004220   ADD r1, r1
0000015C: 00004220   ADD r1, r1
00000160: 00004120   ADD r0, r1
00000164: 00082114   LOAD_IND.S32 r0, r0
00000168: 00000162   PRINT_INT r0
0000016C: 000C0113   LDI r0, data64  data = 0x0000000000000005 (5)
00000178: 00002210   MOV r1, r0
0000017C: 00008110   MOV r0, r3
00000180: 00004220   ADD r1, r1
00000184: 00004220   ADD r1, r1
00000188: 00004120   ADD r0, r1
0000018C: 00082114   LOAD_IND.S32 r0, r0
00000190: 00000162   PRINT_INT r0
00000194: 000C0113   LDI r0, data64  data = 0x0000000000000006 (6)
000001A0: 00002210   MOV r1, r0
000001A4: 00008110   MOV r0, r3
000001A8: 00004220   ADD r1, r1
000001AC: 00004220   ADD r1, r1
000001B0: 00004120   ADD r0, r1
000001B4: 00082114   LOAD_IND.S32 r0, r0
000001B8: 00000162   PRINT_INT r0
000001BC: 000C0113   LDI r0, data64  data = 0x0000000000000007 (7)
000001C8: 00002210   MOV r1, r0
000001CC: 00008110   MOV r0, r3
000001D0: 00004220   ADD r1, r1
000001D4: 00004220   ADD r1, r1
000001D8: 00004120   ADD r0, r1
000001DC: 00082114   LOAD_IND.S32 r0, r0
000001E0: 00000162   PRINT_INT r0
000001E4: 000C0113   LDI r0, data64  data = 0x0000000000000008 (8)
000001F0: 00002210   MOV r1, r0
000001F4: 00008110   MOV r0, r3
000001F8: 00004220   ADD r1, r1
000001FC: 00004220   ADD r1, r1
00000200: 00004120   ADD r0, r1
00000204: 00082114   LOAD_IND.S32 r0, r0
00000208: 00000162   PRINT_INT r0
0000020C: 000C0113   LDI r0, data64  data = 0x0000000000000009 (9)
00000218: 00002210   MOV r1, r0
0000021C: 00008110   MOV r0, r3
00000220: 00004220   ADD r1, r1
00000224: 00004220   ADD r1, r1
00000228: 00004120   ADD r0, r1
0000022C: 00082114   LOAD_IND.S32 r0, r0
00000230: 00000162   PRINT_INT r0
00000234: 000C0113   LDI r0, data64  data = 0x000000000000000A (10)
00000240: 00002210   MOV r1, r0
00000244: 00008110   MOV r0, r3
00000248: 00004220   ADD r1, r1
0000024C: 00004220   ADD r1, r1
00000250: 00004120   ADD r0, r1
00000254: 00082114   LOAD_IND.S32 r0, r0
00000258: 00000162   PRINT_INT r0
0000025C: 000C0113   LDI r0, data64  data = 0x000000000000000B (11)
00000268: 00002210   MOV r1, r0
0000026C: 00008110   MOV r0, r3
00000270: 00004220   ADD r1, r1
00000274: 00004220   ADD r1, r1
00000278: 00004120   ADD r0, r1
0000027C: 00082114   LOAD_IND.S32 r0, r0
00000280: 00000162   PRINT_INT r0
00000284: 000C0113   LDI r0, data64  data = 0x000000000000000C (12)
00000290: 00002210   MOV r1, r0
00000294: 00008110   MOV r0, r3
00000298: 00004220   ADD r1, r1
0000029C: 00004220   ADD r1, r1
000002A0: 00004120   ADD r0, r1
000002A4: 00082114   LOAD_IND.S32 r0, r0
000002A8: 00000162   PRINT_INT r0
000002AC: 00008110   MOV r0, r3
000002B0: 00000071   FREE rZ
000002B4: 000C0113   LDI r0, data64  data = 0x0000000000000000 (0)
000002C0: 00000001   HALT
000002C4: 00000001   HALT
000002C8: 00000001   HALT

[Mini-C Compiler] Количество строк кода: 135
[Mini-C Compiler] Размер файла программы: 716 байт
[Unknown Name Device]
 === ЗАПУСК ПОТОКА СИМУЛЯЦИИ ===
[Unknown Name Device] 5
[Unknown Name Device] 10
[Unknown Name Device] 8
[Unknown Name Device] 0
[Unknown Name Device] 32033
[Unknown Name Device] 786707
[Unknown Name Device] 10
[Unknown Name Device] 0
[Unknown Name Device] 8480
[Unknown Name Device] 8480
[Unknown Name Device] 112
[Unknown Name Device] 9232
[Unknown Name Device] 786707
[Unknown Name Device] Регистр [0]= 0
[Unknown Name Device] Регистр [1]= 0
[Unknown Name Device] Регистр [2]= 48
[Unknown Name Device] Регистр [3]= 10
[Unknown Name Device] Регистр [4]= 0
[Unknown Name Device] Регистр [5]= 0
[Unknown Name Device] Регистр [6]= 0
[Unknown Name Device] Регистр [7]= 0
[Unknown Name Device] Регистр [8]= 0
[Unknown Name Device] Регистр [9]= 0
[Unknown Name Device] Регистр [10]= 0
[Unknown Name Device] Регистр [11]= 0
[Unknown Name Device] Регистр [12]= 0
[Unknown Name Device] Регистр [13]= 0
[Unknown Name Device] Регистр [14]= 0
[Unknown Name Device] Регистр [15]= 0
[Unknown Name Device] Регистр [16]= 0
[Unknown Name Device] Регистр [17]= 0
[Unknown Name Device] Регистр [18]= 0
[Unknown Name Device] Регистр [19]= 0
[Unknown Name Device] Регистр [20]= 0
[Unknown Name Device] Регистр [21]= 0
[Unknown Name Device] Регистр [22]= 0
[Unknown Name Device] Регистр [23]= 0
[Unknown Name Device] Регистр [24]= 0
[Unknown Name Device] Регистр [25]= 0
[Unknown Name Device] Регистр [26]= 0
[Unknown Name Device] Регистр [27]= 0
[Unknown Name Device] Регистр [28]= 0
[Unknown Name Device] Регистр [29]= 1048568
[Unknown Name Device] Регистр [30]= 40
[Unknown Name Device] Регистр [31]= 708
[Unknown Name Device] IP 708
[Unknown Name Device]
 === ЗАВЕРШЕНИЕ (HALT) ===

```
---

