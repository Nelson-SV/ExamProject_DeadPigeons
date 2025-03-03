export enum ErrorMessages {
    FileNotUploaded = `Please upload a file with "jpg" or "png" format`,
    FileFormatUnsupported = `File format needs to be "jpg" or "png"`,
    ValueNotSelected = "Please select a top up value",
    BalanceInvalid = "Please select a value for the top up",
    TransactionId = "Transaction id can not be empty",
    ErrorTransaction = "Please insert a transaction id or upload a picture with the transaction id",
    UpperCharacterContain = "Password must contain at least one upper character",
    LowerCaseContain = "Password must contain at least one lower case character",
    DigitContain = "Password must contain at least  one digit character",
    NonAlphaNumeric = "Password must contain at least  one non-alphanumeric character",
    PasswordEmpty = "Password field can not be empty",
    UserName = "User name can not be empty",
    UserNameTaken = "User name already in use",
    MinLength = "Minimum 6 characters",
    InvalidPassword = "Password invalid please check if is empty",
    EmailInvalid = "Email field can not be empty",
    MaxSequence = "Maximum 3 numbers allowed for the sequence",
    GameInPlace = "Winning numbers can be entered after sunday at 5 a clock!",
    PasswordsMustMatch = "Password confirmation is not the same as password, please retry!"

}

export enum ErrorCodes {
    FileNotUploaded,
    FileFormatUnsupported,
    ValueNotSelected,
    BalanceInvalid,
    TransactionId,
    ErrorTransaction,
    MaxSequence,
    GameInPlace,
    PasswordsMustMatch
}