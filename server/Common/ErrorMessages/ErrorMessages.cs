using Common.SupportedFormat;

namespace Common.ErrorMessages;

public enum ErrorCode
{
    ErrorId,
    ErrorFileLength,
    BalanceInvalid,
    ErrorEmail,
    InvalidPassword,
    UserNotFound,
    UserId,
    UnauthorizedAccess,
    InternalServerError,
    FormatNotSupported,
    FileHandling,
    ErrorRegisterPayment,
    SuccessRegisterPayment,
    BalanceEmpty,
    InsertingTicketsFailed,
    RetrievingBalanceFailed,
    SequenceMissing,
    InsufficientBalance,
    TicketPricesNotFound,
    GameTicketStatusAutomisation,
    NoActiveGames,
    ErrorDeletingTicket,
    PasswordResetSuccess,
    PasswordResetFailed,
    InvalidLogin,
    RetrievingPaymentsFailed,
    DecliningPayment,
    UpdatingPaymentStatusFailed,
    InvalidRole,
    UnexpectedError,
    GameRetrieveError,
    GameIdInvalid,
    InsertedWinningSequence,
    InvalidUserProfile,
    ErrorRetrievePlayers,
    InsertingTicketsFailedCustomer,
    NoTicketsFound,
    FailedToRetrieveAutomatedTickets,
    EmailNotFound,
    PaymentNotFound,
    GameIdNotFound,
    ErrorRetrieveTickets,
    EmailNotSent,
    FailedDeleteUser,
}

public static class ErrorMessages
{
    private static readonly Dictionary<ErrorCode, string> _errorMessages = new()
    {
        { ErrorCode.ErrorId, "Id is required" },
        { ErrorCode.ErrorFileLength, "File can not be empty" },
        { ErrorCode.ErrorEmail, "Email is invalid" },
        { ErrorCode.InvalidPassword, "The password is incorrect,please retry" },
        { ErrorCode.UserNotFound, "User not found" },
        { ErrorCode.UnauthorizedAccess, "Unauthorized access" },
        { ErrorCode.InternalServerError, "An internal server error occurred" },
        {
            ErrorCode.FormatNotSupported,
            $"Please provide a file in the following formats: {SupportedFormats.GetAllFormats()}"
        },
        { ErrorCode.BalanceInvalid, ("Supported values:") },
        { ErrorCode.UserId, "User id can not be empty" },
        { ErrorCode.SuccessRegisterPayment, "Payment registered successfully." },
        { ErrorCode.PasswordResetFailed, "Password reset failed please retry" },
        { ErrorCode.InvalidRole, "Invalid role,please check if role is empty,or the value is valid: [Admin,Player]" },
        { ErrorCode.UnexpectedError, "An unexpected error occured, please try again" },
        { ErrorCode.GameRetrieveError, "An unexpected error occured while retrieving game info" },
        { ErrorCode.GameIdInvalid, "Game id provided is invalid" },
        {    ErrorCode.InsertedWinningSequence,"An error occured while processing the winning numbers"},
        { ErrorCode.FileHandling ,"An error occured while handling your file, please retry!"},
        {ErrorCode.ErrorRegisterPayment,"A error occurred while saving the payment."},
        { ErrorCode.PasswordResetSuccess,"Password reset operation successfully"},
        { ErrorCode.InvalidLogin,"Invalid log in, password or user are invalid , please try again" },
        {ErrorCode.BalanceEmpty,"Does not have a balance record."},
        {ErrorCode.InsertingTicketsFailed,"Could not insert tickets and update balance."},
        {ErrorCode.RetrievingBalanceFailed,"Could not retrieve balance."},
        {ErrorCode. SequenceMissing,"Ticket has no sequence."},
        {ErrorCode. InsufficientBalance, "Insufficient balance"},
        {ErrorCode. TicketPricesNotFound, "Sequence numbers are invalid"},
        {ErrorCode. GameTicketStatusAutomisation, "Could not update game status"},
        {ErrorCode. NoActiveGames, "Could not find active games"},
        {ErrorCode. ErrorDeletingTicket, "Could not proceed with the operation"},
        {ErrorCode. RetrievingPaymentsFailed, "Could not retrieve payments, please retry!"},
        {ErrorCode. DecliningPayment, "Could not decline payment, please retry!"},
        {ErrorCode. UpdatingPaymentStatusFailed, "Could not update payment status, please retry!"},
        {ErrorCode. InvalidUserProfile, "User details invalid to create a user profile, please try again"},
        { ErrorCode.ErrorRetrievePlayers ,"Error while retrieving winning players,please retry"},
        {ErrorCode. InsertingTicketsFailedCustomer, "Could save tickets, please retry!"},
        {ErrorCode. NoTicketsFound, "No automated tickets found "},
        {ErrorCode. FailedToRetrieveAutomatedTickets, "FailedToRetrieveAutomatedTickets "},
        {ErrorCode. EmailNotFound, "Email not found"},
        {ErrorCode. PaymentNotFound, "Payment not found"},
        { ErrorCode.GameIdNotFound, "Game id not found" },
        { ErrorCode.EmailNotSent, "Problem sending email" },
        { ErrorCode.FailedDeleteUser, "Could not proceed with the operation to delete the user"},

        
    };
    
    public static string GetMessage(ErrorCode errorCode)
    {
        return _errorMessages.GetValueOrDefault(errorCode, "This error is undefined");
    }
}

public enum SuccessCode
{
    InsertedWinningNumbers
}

public static class SuccessMessages
{
    private static readonly Dictionary<SuccessCode, string> _successMessages = new()
    {
        { SuccessCode.InsertedWinningNumbers, "Winning sequence processed successfully " }
    };

    public static string GetMessage(SuccessCode successCode)
    {
        return _successMessages.GetValueOrDefault(successCode, "Process successful");
    }
}