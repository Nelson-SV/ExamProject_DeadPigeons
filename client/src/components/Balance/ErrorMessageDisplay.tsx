import {ErrorCodes} from "/src/helpers/ErrorMessages.tsx";

interface ErrorMessages{
    messages: Map<ErrorCodes ,string>
}

export const ErrorMessagesDisplay = ({ messages }: ErrorMessages ) => (
    <div className="flex flex-column items-start justify-start w-full max-w-md">
        {Array.from(messages.entries()).map(([key, value]) => (
            <p key={key} className="text-md text-red-500">
                {value}
            </p>
        ))}
    </div>
);