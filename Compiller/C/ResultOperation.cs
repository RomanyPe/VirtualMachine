namespace Compiller.C;

public enum ResultOperation
{
	// --- Успех ---
	Success = 0,                // Всё прошло отлично (компиляция и запуск успешны)

	// --- Ошибки валидации и инициализации (CreateNewDevice / Load) ---
	InvalidData,                // Некорректные входные данные (null, пустой массив и т.д.)
	BiosNotFound,               // BIOS отсутствует или поврежден
	ProgramNotFound,            // Программа не была скомпилирована или пуста
	OutOfMemory,                // Программа не влезает в выделенный объем RAM

}
