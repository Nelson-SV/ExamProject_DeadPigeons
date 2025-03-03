import {useEffect, useRef, useState} from "react";
import {ErrorMessages, getUserInfoFromToken, http, ValidationError} from "/src/helpers";
import {AxiosResponse} from "axios";
import {UpdatedBalanceResponse} from "/src/Api.ts";
import toast from "react-hot-toast";
import {ErrorMessagesDisplay, InputTransactionNumber} from "./index.ts"
import {ErrorCodes} from "/src/helpers/ErrorMessages.tsx";
import {ErrorResponse} from "helpers/imports.ts";
import {useNavigate} from "react-router-dom";
import {XCircleIcon} from "@heroicons/react/24/solid";

interface BalanceProps {
    topUpValue: number;
}

export const ManageBalance = ({topUpValue}: BalanceProps) => {
    const authUserId: string = getUserInfoFromToken().userId;
    const [serverErrors, setServerErrors] = useState<ValidationError | null>(null);
    const supportedFormats: string = ".jpg,.png";
    const [state, setState] = useState({
        preview: "",
        uploadedFile: new File([""], "empty.png", {type: "image/png"}),
        errorMessage: new Map<ErrorCodes, string>(),
        transactionId: "0",
        showDelete: false
    });
    const navigate = useNavigate();

    useEffect(() => {
        if (topUpValue > 0) {
            removeErrorMessage(ErrorCodes.BalanceInvalid)
        }
    }, [topUpValue]);

    const fileInputRef = useRef<HTMLInputElement | null>(null);

    const validateAndSetError = (condition: any, field: string, message: string, errorCode: ErrorCodes) => {
        if (condition) {
            addErrorMessage(errorCode, message)
            return false;
        }
        return true;
    };

    const handleFileChange = (event) => {
        const file = event.target.files[0];
        if (!validateAndSetError(isFileEmpty(file), "fileFormat", ErrorMessages.FileNotUploaded, ErrorCodes.FileNotUploaded)) {
            return
        } else {
            removeErrorMessage(ErrorCodes.FileNotUploaded);
            removeErrorMessage(ErrorCodes.ErrorTransaction)
        }

        if (!validateAndSetError(!validateFileFormat(file), "fileFormat", ErrorMessages.FileFormatUnsupported, ErrorCodes.FileFormatUnsupported)) {
            const blobUrl = URL.createObjectURL(file);
            setState((prev) => ({
                ...prev,
                uploadedFile: file,
                preview: blobUrl,
                showDelete: true
            }));
            return;
        } else {
            removeErrorMessage(ErrorCodes.FileFormatUnsupported);
            removeErrorMessage(ErrorCodes.ErrorTransaction)
        }

        const blobUrl = URL.createObjectURL(file);
        setState((prev) => ({
            ...prev,
            uploadedFile: file,
            preview: blobUrl,
            showDelete: true,
        }));
    };


    function processPayment() {
        clearErrorMessages();
        http.user
            .paymentUploadImage(state.uploadedFile!, topUpValue, authUserId, state.transactionId)
            .then((response: AxiosResponse<UpdatedBalanceResponse>) => {
                if(response.status==200){
                    toast.success(response.data.message);
                    setTimeout(()=>{
                        navigate("/user/play");
                    },400)
                }
            })
            .catch((e) => {
                const errorData = e.response?.data;
                if (errorData?.errors) {
                    const errors = errorData.errors as ErrorResponse;
                    setServerErrors(errors.errors);
                    if (errorData.errors) {
                        setServerErrors(errorData.errors);
                    } else {
                        toast.error("An unexpected error occurred.");
                    }
                } else {
                    toast.error("Network error. Please try again later.");
                }
                {
                    Object.entries(serverErrors!).map(([field, messages]) => (
                        messages.map((e) =>{
                            if(e!=null){
                                toast.error(e);
                            }
                        })
                    ))
                }
            });
    }

    const updateBalance = () => {
        if (topUpValue === 0) {
            addErrorMessage(ErrorCodes.BalanceInvalid, ErrorMessages.BalanceInvalid);
            return;
        }
        if (
            isTransactionIdStandard(state.transactionId) &&
            isFileEmpty(state.uploadedFile) && !validateFileFormat(state.uploadedFile)
        ) {
            addErrorMessage(ErrorCodes.ErrorTransaction, ErrorMessages.ErrorTransaction)
            return;
        }

        if (!isTransactionIdStandard(state.transactionId)) {
            processPayment();
            return;
        }

        if (isFileEmpty(state.uploadedFile)) {
            addErrorMessage(ErrorCodes.ErrorTransaction, ErrorMessages.ErrorTransaction);
            return;
        }

        if (!isFileEmpty(state.uploadedFile!) && validateFileFormat(state.uploadedFile)) {
            processPayment()
        } else {
            addErrorMessage(ErrorCodes.FileFormatUnsupported, ErrorMessages.FileFormatUnsupported)
        }
    };


    const validateFileFormat = (file) => {
        const extension = file?.name.split(".").pop();
        return supportedFormats.includes(`.${extension}`);
    };

    const isFileEmpty = (file: File | null) => file === null || file.size === 0;

    const isTransactionIdStandard = (transactionId: string) => {
        return transactionId==="0";
    }

    const getTransactionId = (transactionId: string) => {
        clearErrorMessages()
        let processTransaction=transactionId;
        if(transactionId.length===0){
            processTransaction="0";
        }
        setState((prev) => ({
            ...prev,
            transactionId: processTransaction,
        }));
    }

    const handleReset = () => {
        removeErrorMessage(ErrorCodes.FileFormatUnsupported);
        removeErrorMessage(ErrorCodes.ErrorTransaction);
        setState((prev) => ({
            ...prev,
            uploadedFile: new File([""], "empty.png", {type: "image/png"}),
            preview: "",
            showDelete: false,
        }));
        if (fileInputRef.current) {
            fileInputRef.current.value = "";
        }
    }

    const addErrorMessage = (errorCode: ErrorCodes, errorMessage: string) => {
        setState((prev) => {
            const newErrorMessage = new Map(prev.errorMessage);
            newErrorMessage.set(errorCode, errorMessage);
            return {
                ...prev,
                errorMessage: newErrorMessage,
            };
        });
    };

    const removeErrorMessage = (errorCode: ErrorCodes) => {
        setState((prev) => {
            const newErrorMessage = new Map(prev.errorMessage);
            newErrorMessage.delete(errorCode);
            return {
                ...prev,
                errorMessage: newErrorMessage,
            };
        });
    };

    const clearErrorMessages = () => {
        setState((prev) => {
            const newErrorMessage = new Map(prev.errorMessage);
            newErrorMessage.clear();
            return {
                ...prev,
                errorMessage: newErrorMessage,
            };
        });
    }


    return (
        <div className="flex flex-col items-center justify-center gap-2 mt-10 space-y-1 ">
            <InputTransactionNumber setDisabled={state.uploadedFile.size != 0}
                                    getTransactionNumber={getTransactionId}></InputTransactionNumber>
            <div className="flex flex-row items-center justify-center  w-full max-w-md">
                <input type="file"
                       ref={fileInputRef}
                       disabled={state.transactionId!="0"} accept={`${supportedFormats}`}
                       onChange={handleFileChange}
                       className="file-input file-input-bordered w-full max-w-xs"/>
                <button
                    onClick={handleReset}
                    className={`px-3 py-2 flex items-center justify-center ${
                        state.showDelete ? "text-red-500 hover:text-black" : "text-blue-400 disabled"
                    } focus:outline-none`}
                    title="Clear Input"
                >
                    <XCircleIcon className="h-5 w-5" />
                </button>
            </div>
            <ErrorMessagesDisplay messages={state.errorMessage}></ErrorMessagesDisplay>
            <button onClick={() => updateBalance()}
                    className="w-36 h-15 text-white py-2 bg-black font-semibold rounded-md border border-transparent hover:bg-red-500 hover:text-white hover:border-black transition-colors duration-300 text-center">
                Køb
            </button>
            {state.preview.length !== 0 && (
                <div className="w-full max-w-md mx-auto">
                    <img
                        src={state.preview}
                        alt="Preview"
                        className="w-full h-auto object-contain rounded"
                    />
                </div>
            )
            }

        </div>

    )


}
