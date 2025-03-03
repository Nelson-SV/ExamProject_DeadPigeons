import SequenceDisplay from "/src/components/user/historyPage/SequenceDisplay.tsx";
import {CurrentGameDto} from "/src/Api.ts";
import {useNavigate} from "react-router-dom";

interface GameDisplay {
    currentGame: CurrentGameDto
    index: number
}


export const SearchResponseGameDisplay = ({currentGame, index}: GameDisplay) => {
    const navigate =useNavigate();
    const setResult = () => {
        navigate(`/admin/history/${currentGame.guid}`);
    }

    return (
        <div
            className={`p-3 flex flex-row flex-wrap justify-around items-center gap-2 border border-blue-300 rounded-md  ${index % 2 === 0 ? "bg-border" : "bg-gray-500" } hover:bg-blue-300 hover:scale-101 transition-transform duration-200 `
            }
            onClick={setResult}>
            <h3 className={"text-hover_text"}>{currentGame.formattedStartDate}</h3>
            <SequenceDisplay numbers={currentGame.extractedNumbers}
                             winningNumbers={currentGame.extractedNumbers}></SequenceDisplay>
        </div>
    )
}
