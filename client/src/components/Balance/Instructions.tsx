import {useAtom} from "jotai";
import {InstructionsMessages} from "/src/atoms";
import {InstructionCommand} from "/src/components/Balance/InstructionCommand.tsx";


export const TopUpInstructions =()=>{
    const[instructions,]= useAtom(InstructionsMessages);
return (
    <div className="flex flex-col items-center justify-center bg-instructions_background sm:w1/2  md:w-2/4 lg:w-3/4 xl:w-2/4  mx-auto">
            <h2 className={"text-3xl font-bold mb-4"}>Følg disse trin for at fuldføre din påfyldning:</h2>
        <ol className={"list-disc pl-8 space-y-4 text-lg leading-relaxed"}>
            {instructions.map((i)=>(
                <InstructionCommand key={i} message={i}></InstructionCommand>
            ))}
        </ol>
    </div>
)
}


