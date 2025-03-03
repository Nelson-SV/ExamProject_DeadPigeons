import {
    BalanceAtom,
    DisabledSequenceInput,
    DisplayWiningSequence,
    WinningNumbersPannel,
    WinningTicketsDisplay
} from "/src/components/imports";
import {useAtom} from "jotai";
import {useEffect, useState} from "react";
import {CurrentGameDto} from "/src/Api.ts";
import {ErrorResponse, http, StorageKeys, useStorageHandler, ValidationError} from "/src/helpers";
import {AxiosResponse} from "axios";
import toast from "react-hot-toast";


export const AdminMainPage = () => {
    const [gameRevenue, setGameRevenue] = useAtom(BalanceAtom);
    const [serverErrors, setServerErrors] = useState<ValidationError | null>(null);
    const [maxInputSequence, setMaxInputSequence] = useState<boolean>(false);
    const sessionStorageHandler = useStorageHandler;
    const [inputDisabled,setInputDisabled] =  useAtom(DisabledSequenceInput);

    useEffect(() => {
        const getGameInfo = async () => {
            var currentGame = sessionStorageHandler.getItem(StorageKeys.GAME_INFO);
            if (!currentGame) {
                await http.api.gameHandlerGetCurrentGamInfo()
                    .then((r: AxiosResponse<CurrentGameDto>) => {
                        if (r.status === 200) {
                            console.log(StorageKeys.GAME_INFO)
                            sessionStorageHandler.setItem(StorageKeys.GAME_INFO
                                ,r.data);
                            setInputDisabled({disabled:r.data.status});
                            setGameRevenue({balanceValue: r.data.revenue!});
                        }
                    }).catch((e) => {
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
                        if (serverErrors) {
                            Object.entries(serverErrors).forEach(([field, messages]) => {
                                if (messages) {
                                    messages.forEach((message) => {
                                        if (message) {
                                            toast.error(message);
                                        }
                                    });
                                }
                            });
                        }

                    });
            }
        }
        getGameInfo();
    }, []);

    const setMaxReached = (isMaxReached: boolean) => {
        setMaxInputSequence(isMaxReached);
    }

    return (
        <div className="flex flex-grow container mx-auto py-8">
            <main className="w-full p-4 mx-auto">
                <div
                    className="flex flex-col justify-start items-center w-full  bg-background rounded-lg p-5 shadow-lg lg:w-3/4 xl:w-3/5 mx-auto md:p-10">
                    <WinningNumbersPannel setMaxReached={setMaxReached}></WinningNumbersPannel>
                    <DisplayWiningSequence isMaxInputReached={maxInputSequence}
                                           setMaxReached={setMaxReached}
                    >
                    </DisplayWiningSequence>
                </div>
                <div>
                    <WinningTicketsDisplay></WinningTicketsDisplay>
                </div>
            </main>
        </div>
    )
}