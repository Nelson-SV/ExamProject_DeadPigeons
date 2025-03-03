import {DisabledSequenceInput, Loading, SequenceButton} from "/src/components/imports.ts";
import {useEffect, useState} from "react";
import {useAtom} from "jotai";


interface PannelState {
    setMaxReached: (isMaxReached: boolean) => void,
}

export const WinningNumbersPannel = ({setMaxReached}: PannelState) => {
    const values: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16];
    const [disabledInput,_] = useAtom(DisabledSequenceInput);
    const [isHydrated, setHydrated] = useState(false);

    useEffect(() => {
        if (disabledInput !== null) {
            setHydrated(true);
        }
    }, [disabledInput]);

    if (!isHydrated || disabledInput === null) {
        return <Loading/>;
    }

    return (
        <div className={"grid  grid-cols-4 grid-rows-4 gap-5 w-3/4 h-full md:gap-10    "}>
            {values.map((v) => {

                return (
                    <SequenceButton inputDisabled={disabledInput.disabled} setMaxReached={setMaxReached}
                                    key={v} value={v}
                                    buttonId={v + ""}></SequenceButton>
                )
            })
            }
        </div>
    )


}