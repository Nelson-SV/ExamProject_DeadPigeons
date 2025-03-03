import { XCircleIcon } from "@heroicons/react/24/solid";
import { useState } from "react";

interface InputParams {
    getTransactionNumber: (value: string) => void;
    setDisabled: boolean;
}

export const InputTransactionNumber = ({ getTransactionNumber, setDisabled }: InputParams) => {
    const [isVisible, setIsVisible] = useState(false);
    const [transactionId,setTransactionId] =  useState<string>("");
    const handleChange = (value: string) => {
        setIsVisible(value.length > 0);
        setTransactionId(value);
        getTransactionNumber(value);
    };

    const handleReset = () => {
        getTransactionNumber("0");
        setTransactionId("");
        setIsVisible(false);
    };

    return (
        <div className="flex items-center justify-center w-full max-w-md  border-red-500 rounded-md ">
            <input
                type="text"
                disabled={setDisabled}
                value={transactionId}
                onChange={(e) => handleChange(e.target.value)}
                placeholder="Indsæt mobilpay transaktionsnummer"
                className="flex-1 px-3 py-2 outline-none rounded-md text-gray-800 disabled:cursor-not-allowed  disabled:bg-transparent disabled:text-gray-200 w-full max-w-xs"
            />

            <button
                onClick={handleReset}
                className={`px-3 py-2 flex items-center justify-center ${
                    isVisible ? "text-red-500 hover:text-black" : "text-blue-400 disabled"
                } focus:outline-none`}
                title="Clear Input"
            >
                <XCircleIcon className="h-5 w-5" />
            </button>
        </div>
    );
};
