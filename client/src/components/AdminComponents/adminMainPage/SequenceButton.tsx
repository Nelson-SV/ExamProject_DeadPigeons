import {useAtom} from "jotai";
import {WinningSequenceAtom} from "/src/atoms";

interface TopUp {
    value: number,
    buttonId: string,
    setMaxReached: (isReached: boolean) => void
    inputDisabled: boolean
}

export const SequenceButton = ({value, buttonId, setMaxReached,inputDisabled}: TopUp) => {
    const [currentSequence, setWinningSequence] = useAtom(WinningSequenceAtom);
    const styleNormal = "btn btn-outline rounded-md bg-background disabled:bg-gray-200 disabled:cursor-not-allowed disabled:text-hover_text ";
    const styleSelected = "btn btn-outline rounded-md hover:bg-hover text-hover_text  border-1 border-border  bg-selected_btn  disabled:bg-gray-300 disabled:cursor-not-allowed   !text-hover_text"

    const isSelected = currentSequence.includes(Number.parseInt(buttonId));
    const buttonStyle = `${styleNormal} ${isSelected ? styleSelected : ""}`;
    const retrieveValue = (selectedValue: number) => {
        if (currentSequence.length === 3) {
            setMaxReached(true);
            return;
        }
        const updatedSequence = new Set([...currentSequence]);
        updatedSequence.add(selectedValue);
        setWinningSequence(Array.from(updatedSequence));
        setMaxReached(false);
    };

    const removeValue = (selectedValue: number) => {
        const updatedSequence = new Set([...currentSequence]);
        updatedSequence.delete(selectedValue);
        setWinningSequence(Array.from(updatedSequence));
    }

    const buttonAction = (isSelected: boolean, selectedValue: number) => {
        if (isSelected) {
            removeValue(selectedValue);
            return;
        }
        retrieveValue(selectedValue);
    }


    return (
        <button disabled={inputDisabled} id={buttonId} onClick={(e) => {
            buttonAction(isSelected, value);
        }} className={`${buttonStyle}`}>
            <p>{value}</p>
        </button>
    )


}


