interface EmailInputParams {
    getInputValue: (email: string) => void;
    placeholder: string;
    value: string;
}

export const EmailInput = ({ getInputValue, placeholder, value }: EmailInputParams) => {
    return (
        <div className="input input-bordered flex items-center gap-2">
            <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 16 16"
                fill="currentColor"
                className="h-4 w-4 opacity-70"
            >
                <path
                    d="M0 4a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V4Zm2-.5a.5.5 0 0 0-.5.5v.217l6 3.6 6-3.6V4a.5.5 0 0 0-.5-.5H2Zm12 2.383-5.554 3.333a.5.5 0 0 1-.492 0L2 5.883V12a.5.5 0 0 0 .5.5h11a.5.5 0 0 0 .5-.5V5.883Z"
                />
            </svg>
            <input
                onChange={(e) => getInputValue(e.target.value)}
                type="email"
                className="grow"
                placeholder={placeholder}
                value={value}
            />
        </div>
    );
};
