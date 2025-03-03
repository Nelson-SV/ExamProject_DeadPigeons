import {useState} from "react";
import {PlayerTicketsRequestDto, PlayerTicketsResponseDto, WinningPlayer} from "/src/Api.ts";
import {http, showErrorToasts} from "/src/helpers";
import {DisplayTicket, Loading} from "/src/components/imports.ts";
import {AxiosResponse} from "axios";
import toast from "react-hot-toast";
import {useAtom} from "jotai";

interface PlayerInfo {
    playerInfo: WinningPlayer
    winningNumbers:number[]
}


export const WinningPlayerDisplay = ({playerInfo,winningNumbers}: PlayerInfo) => {
    const [isOpen, setIsOpen] = useState(false);
    const [player, setPlayer] = useState<WinningPlayer>(playerInfo);
    const [winningTickets, setWinningTickets] = useState<PlayerTicketsResponseDto>(
        {playerTickets: []});

    const switchClasses = () => {
        if (isOpen) {
            setIsOpen(false);
            return;
        }
        retrieveTicketsData();
        setIsOpen(true);
    }

    const retrieveTicketsData = () => {
        const playerRequest: PlayerTicketsRequestDto = {playerTicketsIds: playerInfo.winningTicketsIds}
        http.api.gameHandlerGetWinningTicketsForPlayer(playerRequest)
            .then((r: AxiosResponse<PlayerTicketsResponseDto>) => {
                if (r.status === 200) {
                    setWinningTickets(r.data);
                }
            }).catch((e) => {
            const errorData = e.response?.data;
            if (errorData?.errors) {
                showErrorToasts(errorData);
            } else {
                toast.error("Network error. Please try again later.");
            }
        });
    }


    return (
        <div className="join join-vertical w-full rounded-md  mx-auto mb-4">
            <div
                className={`collapse ${isOpen ? "collapse-open" : "collapse-closed"} collapse-arrow join-item border-base-300 border bg-white rounded-md shadow-md transition-all`}
                onClick={switchClasses}
            >
                <div
                    className="collapse-title text-xl font-medium p-4 flex flex-col md:flex-row justify-start gap-4 items-start"
                >
                    <p
                        className="text-gray-600 w-full md:w-1/3 font-bold truncate p-2 rounded text-sm sm:text-base md:text-lg lg:text-xl"
                        title={player.userName!}
                    >
                        Name: {player.userName}
                    </p>
                    <p
                        className="text-gray-600 w-full md:w-1/3  truncate p-2 rounded text-sm sm:text-base md:text-lg lg:text-xl"
                        title={player.email!}
                    >
                        Email: {player.email}
                    </p>
                    <p
                        className="text-gray-600 w-full md:w-1/3  truncate p-2 rounded text-sm sm:text-base md:text-lg lg:text-xl"
                        title={`Winning Tickets: ${player.winningCount}`}
                    >
                        Winning Tickets: <span className={"font-bold"}>{player.winningCount}</span>
                    </p>
                </div>
                {isOpen && (
                    <div className="collapse-content p-4 fles flex-row items-start justify-start ">
                        {winningTickets.playerTickets!.length > 0 ?
                            (winningTickets.playerTickets!.map((t) =>
                                (<DisplayTicket winningNumbers={winningNumbers} key={t.guid} ticket={t}/>))) : ("No available data")}
                    </div>
                )}
            </div>
        </div>
    );
}