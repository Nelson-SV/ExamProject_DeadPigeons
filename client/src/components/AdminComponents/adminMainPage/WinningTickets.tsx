import {TicketsResponseDto} from "/src/Api.ts";
import SequenceDisplay from "/src/components/user/historyPage/SequenceDisplay.tsx";
import {useAtomValue} from "jotai";
import {WinningSequenceAtom} from "/src/atoms";

interface WinningTicketDisplay {
    ticket: TicketsResponseDto
    winningNumbers:number[]
}


export const WinningTicketDisplay = ({ticket,winningNumbers}: WinningTicketDisplay) => {
    const winningSequence = useAtomValue(WinningSequenceAtom);
    return (
        <div className={"text-xl font-medium p-4 flex flex-col md:flex-row justify-start gap-4 items-center bg-gray-500 rounded-lg  shadow-lg mt-2"}>
            <p
                className="text-white w-full md:w-1/3 font-semibold truncate p-2 rounded text-sm sm:text-base md:text-lg lg:text-xl"
                title={ticket.formattedPurchaseDate}
            >
                Purchased: {ticket.formattedPurchaseDate}
            </p>
            <p
                className="text-white w-full md:w-1/3  truncate p-2 rounded text-sm sm:text-base md:text-lg lg:text-xl"
                title={ticket.priceValue + ""}
            >
                Price: {ticket.priceValue}
            </p>
            <SequenceDisplay numbers={ticket.playedNumbers} winningNumbers={winningNumbers.length>0?winningNumbers:winningSequence}></SequenceDisplay>
        </div>

    )
}