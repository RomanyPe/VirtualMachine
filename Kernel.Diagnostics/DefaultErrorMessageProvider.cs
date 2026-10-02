using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Kernel.Diagnostics;

public class DefaultErrorMessageProvider : IErrorMessageProvider
{
    private readonly FrozenDictionary<ErrorCode, string> _templates = new Dictionary<ErrorCode, string>()
    {
        { ErrorCode.NotSupported, "Operation not supported: {0}" },
        { ErrorCode.NotImplemented, "Feature not implemented: {0}" },

        { ErrorCode.Lexer_UnexpectedCharacter, "Unexpected character '{0}' at line {1}, column {2}" },

        { ErrorCode.Parser_UnknownType, "Unknown type '{0}'" },
        { ErrorCode.Parser_ExpectedToken, "Expected {0}, got {1} at {2}:{3}" },
        { ErrorCode.Parser_UnexpectedToken, "Unexpected token: {0}" },
        { ErrorCode.Parser_InvalidAssignment, "Invalid assignment target" },
        { ErrorCode.Parser_ArraySizeNotConstant, "Array size must be constant" },
        { ErrorCode.Parser_UnknownDirective, "Unknown preprocessor directive: #{0}" },
        { ErrorCode.Parser_StructNotDefined, "Struct '{0}' is not defined" },
        { ErrorCode.Parser_InvalidCharLiteral, "Char literal '{0}' is invalid" },

        { ErrorCode.CodeGen_UnknownTypeSize, "Unknown type '{0}' for memory size" },
        { ErrorCode.CodeGen_UnknownOpCodeSize, "Unknown opcode size: {0}" },
        { ErrorCode.CodeGen_UnknownStructType, "Unknown struct type '{0}'" },
        { ErrorCode.CodeGen_UnknownField, "Field '{0}' not found in struct '{1}'" },
        { ErrorCode.CodeGen_NonStructAccess, "'{0}' is not a struct or pointer to struct" },
        { ErrorCode.CodeGen_InvalidDotAccess, "Dot access only for simple variables" },
        { ErrorCode.CodeGen_UndefinedVariable, "Undefined variable '{0}'" },
        { ErrorCode.CodeGen_CannotGetRegisterAddress, "Cannot take address of register variable '{0}'" },
        { ErrorCode.CodeGen_NonIndexableType, "'{0}' is not an array or pointer" },
        { ErrorCode.CodeGen_UnknownFunction, "Function '{0}' not found" },
        { ErrorCode.CodeGen_ArgumentCountMismatch, "Argument count mismatch for function '{0}'" },
        { ErrorCode.CodeGen_ParameterNotInRegister, "Parameter '{0}' not allocated to a register" },
        { ErrorCode.CodeGen_UnknownPointedType, "Cannot determine pointed type for dereference" },
        { ErrorCode.CodeGen_DirectStructLoadStore, "Direct load/store of struct variable '{0}' is not supported" },

        { ErrorCode.Asm_UndefinedLabel, "Undefined label '{0}' at position '{1}'" },
        { ErrorCode.Asm_DuplicateLabel, "Label '{0}' already defined at position {1}" },
        { ErrorCode.Asm_InvalidInstruction, "Invalid instruction syntax: {0}" },
        { ErrorCode.Asm_UnknownRegister, "Unknown register '{0}'" },
        { ErrorCode.Asm_UnknownMnemonic, "Unknown mnemonic '{0}'" },
        { ErrorCode.Lexer_InvalidNumber, "Invalid number literal '{0}' at {1}:{2}" },
        { ErrorCode.Lexer_UnterminatedString, "Unterminated string starting at {0}:{1}" }
    }.ToFrozenDictionary();

    public string GetMessage(ErrorCode code, params ReadOnlySpan<object> args)
    {
        if (!_templates.TryGetValue(code, out var template))
            throw new InvalidOperationException($"No message template for error code {code}");
        return string.Format(template, args);
    }
}


public enum ErrorCode
{
    // Общие
    NotSupported = 1000,
    NotImplemented = 1001,

    // Лексер
    Lexer_UnexpectedCharacter = 1100,
    Lexer_InvalidNumber = 1101,
    Lexer_UnterminatedString = 1102,

    // Парсер
    Parser_UnknownType = 1200,
    Parser_ExpectedToken = 1201,
    Parser_UnexpectedToken = 1202,
    Parser_InvalidAssignment = 1203,
    Parser_ArraySizeNotConstant = 1204,
    Parser_UnknownDirective = 1205,
    Parser_StructNotDefined = 1206,
    Parser_InvalidCharLiteral = 1207,

    // Генератор кода (CodeGen)
    CodeGen_UnknownTypeSize = 1300,
    CodeGen_UnknownOpCodeSize = 1301,
    CodeGen_UnknownStructType = 1302,
    CodeGen_UnknownField = 1303,
    CodeGen_NonStructAccess = 1304,
    CodeGen_InvalidDotAccess = 1305,
    CodeGen_UndefinedVariable = 1306,
    CodeGen_CannotGetRegisterAddress = 1307,
    CodeGen_NonIndexableType = 1308,
    CodeGen_UnknownFunction = 1309,
    CodeGen_ArgumentCountMismatch = 1310,
    CodeGen_ParameterNotInRegister = 1311,
    CodeGen_UnknownPointedType = 1312,
    CodeGen_DirectStructLoadStore = 1313,

    // Ассемблер
    Asm_UndefinedLabel = 1400,
    Asm_DuplicateLabel = 1401,
    Asm_InvalidInstruction = 1402,
    Asm_UnknownRegister = 1403,
    Asm_UnknownMnemonic = 1404,
}


public interface IErrorMessageProvider
{
    string GetMessage(ErrorCode code, params ReadOnlySpan<object> args);
}

public static class ThrowHelper
{
    private static readonly ErrorCode[] _errorCodesValidateTable = Enum.GetValues<ErrorCode>();

    public static IErrorMessageProvider ErrorMessageProvider { get; set; } = new DefaultErrorMessageProvider();

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ThrowMiniC(ErrorCode code, params ReadOnlySpan<object> args)
    {
        var message = ErrorMessageProvider.GetMessage(code, args);
        throw new CodeExpection(code, message);
    }

    [DoesNotReturn]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ThrowMiniC<T>(ErrorCode code, params ReadOnlySpan<object> args)
    {
        var message = ErrorMessageProvider.GetMessage(code, args);
        throw new CodeExpection(code, message);
    }

    public static void ValidateAllCodesCovered(IReadOnlyDictionary<ErrorCode, string> templates)
    {
        if (_errorCodesValidateTable.Any(code => !templates.ContainsKey(code)))
        {
            ErrorCode firstMissing = _errorCodesValidateTable.First(code => !templates.ContainsKey(code));
            throw new InvalidOperationException($"Missing message template for error code {firstMissing}");
        }
    }

    extension(IReadOnlyDictionary<ErrorCode, string> templates)
    {
        public bool TryValidateAllCodesCovered(out ErrorCode code)
        {
            if (_errorCodesValidateTable.Any(code => !templates.ContainsKey(code)))
            {
                code = _errorCodesValidateTable.First(code => !templates.ContainsKey(code));
                return false;
            }
            code = default;
            return true;
        }
    }
}


public class CodeExpection(ErrorCode errorCode, string message) : Exception(message)
{
    public ErrorCode ErrorCode { get; } = errorCode;
    public int NumericCode => (int)ErrorCode;
}