interface DisplaySequence {
    isWinning: boolean,
    value: string
}


export const DisplaySequenceNumber = ({isWinning, value}: DisplaySequence) => {

    return (
        <div
            className={`w-8 h-8 flex items-center justify-center rounded-3xl font-bold text-black 
                        ${isWinning
                ? "hover:shadow-md bg-gradient-to-r from-red-200 to-red-500"
                : "hover:shadow-md bg-gradient-to-r from-gray-100 to-gray-300"}`}
        >
            {value}
        </div>
    )
}