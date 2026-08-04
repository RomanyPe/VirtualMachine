// std.asm – Standard library for Mini-C VM
// Provides essential system calls and utility functions

// -------------------------------------------------------------------
// print_int – prints an integer value stored in r0
// Destroys: r0, r1, r2 (scratch)
// -------------------------------------------------------------------
// func_print_int:
//     PRINT_INT r0
//     RET

// -------------------------------------------------------------------
// _out_port – writes a byte to a port
// Arguments: r0 = port, r1 = value
// Destroys: r0, r1, r2 (scratch)
//-------------------------------------------------------------------
// func__out_port:
//     OUT r1, r0
//     RET

// -------------------------------------------------------------------
// _in_port – reads a byte from a port
// Arguments: r0 = port
// Returns: r0 = value read
// Destroys: r0, r1, r2 (scratch)
// -------------------------------------------------------------------
// func__in_port:
//     IN r0, r0
//     RET

// -------------------------------------------------------------------
// exit – immediately halts the processor (exit program)
// Destroys: nothing
// -------------------------------------------------------------------
// func_exit:
//     HALT
//     RET

// Установка обработчика прерывания
// r0 – номер вектора, r1 – адрес обработчика
func_set_int_vector:
    LDI r2, 0x200
    MOV r3, r0
    ADD r3, r3
    ADD r3, r3     // умножаем на 8
    ADD r2, r3
    STORE.S64 r1, r2
    RET

// Системные вызовы через INT
// print_int через INT 0

func_print_int:
    INT 0
    RET

func__out_port:
    INT 1
    RET

func__in_port:
    INT 2
    RET

func_exit:
    HALT
    RET

// Обработчик INT 0 (print_int)
int0_handler:
    PRINT_INT r0
    IRET