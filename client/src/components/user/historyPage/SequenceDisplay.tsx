
const SequenceDisplay = ({ numbers, winningNumbers }) => {

    const sequence = Array.isArray(numbers) ? numbers : [];
    return (
        <div className="flex gap-2">
            {sequence.map((num, idx) => {
                const isWinningNumber = winningNumbers.includes(num); // Check if the number is a winning number
                return (
                    <div
                        key={idx}
                        className={`w-8 h-8 flex items-center justify-center rounded-3xl font-bold text-black 
                        ${isWinningNumber
                            ? "hover:shadow-md bg-gradient-to-r from-red-200 to-red-500"
                            : "hover:shadow-md bg-gradient-to-r from-gray-100 to-gray-300"}`}
                    >
                        {num}
                    </div>
                );
            })}
        </div>
    );
};

export default SequenceDisplay;
