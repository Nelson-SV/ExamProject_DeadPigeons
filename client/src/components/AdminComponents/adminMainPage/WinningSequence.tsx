import {DisabledSequenceInput, DisplaySequenceNumber, Loading} from "../../imports.ts";
import {useEffect, useState} from "react";
import {useAtom} from "jotai";
import {WinningSequenceAtom, WinningSequenceResponse} from "/src/atoms";
import {ErrorMessagesDisplay} from "/src/components/Balance";
import {ErrorCodes} from "../../../helpers/ErrorMessages.tsx";
import {
    ErrorMessages,
    showErrorToasts,
    StorageKeys,
    useStorageHandler,
    ValidationError
} from "/src/helpers";
import {ProcessedWinningSequence, WinningSequenceDto} from "/src/Api.ts";
import {http} from "/src/helpers"
import {AxiosResponse} from "axios";
import toast from "react-hot-toast";
import {AdminWiningTicketsPaginationAtom} from "/src/atoms/adminAtoms/AdminWiningTicketsPaginationAtom.ts";


interface DisplaySequence {
    isMaxInputReached: boolean
    setMaxReached: (isReached: boolean) => void
}

export const DisplayWiningSequence = ({isMaxInputReached, setMaxReached}: DisplaySequence) => {
    const [winningSequence, setWinningSequence] = useAtom(WinningSequenceAtom);
    const isVisible = winningSequence.length > 0;
    const [errorMessages, setErrorMessages] = useState<Map<ErrorCodes, string>>(new Map());
    const [, setWinningSequenceResponse] = useAtom(WinningSequenceResponse);
    const storageHandler = useStorageHandler;
    const [serverErrors, setServerErrors] = useState<ValidationError | null>(null);
    const [inputDisabled, setInputDisabled] = useAtom(DisabledSequenceInput);
    const [currentPagination, setCurrentPagination] = useAtom(AdminWiningTicketsPaginationAtom);
    const [isHydrated, setHydrated] = useState(false);


    useEffect(() => {
        const currentGame = storageHandler.getItem(StorageKeys.GAME_INFO);
        if (inputDisabled !== null) {
            setHydrated(true);
        }

        if (currentGame != null) {
            if (currentGame.status) {
                setErrorMessages((prevMessages) => {
                    const updatedMap = new Map(prevMessages);
                    updatedMap.set(ErrorCodes.GameInPlace, ErrorMessages.GameInPlace);
                    return updatedMap;
                });
            }
        }

        if (isMaxInputReached) {
            setErrorMessages((prevMessages) => {
                const updatedMap = new Map(prevMessages);
                updatedMap.set(ErrorCodes.MaxSequence, ErrorMessages.MaxSequence);
                return updatedMap;
            });
            const timeout = setTimeout(() => {
                setErrorMessages((prevMessages) => {
                    const updatedMap = new Map(prevMessages);
                    updatedMap.delete(ErrorCodes.MaxSequence);
                    return updatedMap;
                });
                setMaxReached(false);
            }, 500);

            return () => clearTimeout(timeout);
        }
    }, [isMaxInputReached, inputDisabled]);

    const handleReset = () => {
        setWinningSequence([]);
    }

    const registerWinningSequence = () => {
        const currentGame = storageHandler.getItem(StorageKeys.GAME_INFO);
        let insertRequest: WinningSequenceDto = {winningSequence: winningSequence, gameId: currentGame!.guid}


        http.api.gameHandlerSetWinningNumbers(insertRequest)
            .then((r: AxiosResponse<ProcessedWinningSequence>) => {
                    if (r.data) {
                        setCurrentPagination({
                            currentPageItems: 10,
                            currentPage: 1,
                            nextPage: 2,
                            hasNext: false,
                            totalItems: 0
                        });
                        setWinningSequenceResponse(r.data);
                        setServerErrors(null);
                        setInputDisabled({disabled: true})
                        toast.success(r.data.message);
                    }
                }
            ).catch((e) => {
            const errorData = e.response?.data;
            if (errorData?.errors) {
                setServerErrors(errorData.errors);
                showErrorToasts(errorData);
            } else {
                toast.error("Network error. Please try again later.");
            }
        });
    }


    if (!isHydrated) {
        return (<Loading></Loading>)
    }

    return (
        <>
            <div className={"mt-5 flex flex-row flex-wrap justify-center items-center w-3/4 gap-2 h-full "}>
                {
                    [...winningSequence].map((w) => (
                        <DisplaySequenceNumber value={w + ""} isWinning={true} key={w + ""}></DisplaySequenceNumber>))
                }
            </div>
            {(serverErrors != null || errorMessages) &&
                (<div className={"p-1 h-10 "}>
                    <ErrorMessagesDisplay messages={errorMessages}/>
                </div>)}

            {isVisible && (
                <div
                    className={"mt-5 flex flex-row flex-wrap justify-center items-center w-3/4 gap-2 h-full md:justify-end"}>
                    <button
                        disabled={inputDisabled.disabled}
                        onClick={handleReset}
                        className={"px-3 gap-1 flex items-center justify-center w-36 h-15 text-white py-2 bg-black font-semibold rounded-md border border-transparent hover:bg-red-500 hover:text-white hover:border-black transition-colors duration-300 text-center disabled:bg-gray-300 disabled:text-gray-200 disabled:cursor-not-allowed  disabled:border-transparent"}
                        title="Clear Input"
                    >
                        <p className={"display block"}>Slette</p>
                    </button>

                    <button disabled={inputDisabled.disabled} onClick={() => registerWinningSequence()}
                            className="w-36 h-15 text-white py-2 bg-black font-semibold rounded-md border border-transparent hover:bg-red-500 hover:text-white hover:border-black transition-colors duration-300 text-center disabled:cursor-not-allowed  disabled:bg-gray-300 disabled:text-gray-200  disabled:border-transparent">
                        Indsende
                    </button>
                </div>
            )}
        </>
    )
}


