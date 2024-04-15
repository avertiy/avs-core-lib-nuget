namespace AVS.CoreLib.Extensions.Enums;

public enum StringCase
{
    None = 0,
    UPPER_CASE,     // e.g., MAX_LENGTH
    lower_case,     // e.g., MAX_LENGTH
    PascalCase,     // e.g., MyVariableName
    CamelCase,      // e.g., myVariableName
    snake_case,     // e.g., my_variable_name
    kebab_case,     // e.g., my-variable-name        
    TrainCase,      // e.g., My-Variable-Name
    MixedCase,      // e.g., iPhone, McDonald's
    TitleCase,      // e.g., The Lord of the Rings
    ScreamingSnakeCase  // e.g., WARNING_MESSAGE
}