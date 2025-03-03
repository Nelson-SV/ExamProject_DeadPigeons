interface TopUp {
    value: number,
    getValue: (value: number) => void,
    buttonId: string
    setSelectedButtonId: (value: string) => void;
    getSelectedButton: (selectedButtonId:string) => string;
}

export const TopUpValue = ({value, getValue, buttonId, setSelectedButtonId, getSelectedButton}: TopUp) => {
    const styleNormal = "btn btn-outline rounded-md bg-background ";
    const styleSelected = "btn btn-outline rounded-md hover:bg-hover text-hover_text  border-1 border-border  bg-selected_btn  !text-hover_text"
    const isSelected = getSelectedButton(buttonId) === buttonId;
    const buttonStyle = `${styleNormal} ${isSelected ? styleSelected : ""}`;

    return (
        <button id={buttonId} onClick={(e) => {
            getValue(value);
            setSelectedButtonId(e.currentTarget.id);
        }} className={`${buttonStyle}`}>
            <p>{value}</p>
        </button>
    )


}








