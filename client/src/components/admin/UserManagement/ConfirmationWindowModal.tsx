
export default function ConfirmationWindowModal({ isOpen, title, message, onConfirm, onCancel }) {
    if (!isOpen) return null;

    return (
        <dialog className="modal bg-opacity-60 bg-black" open>
            <div className="modal-box">
                <h3 className="font-bold text-lg">{title}</h3>
                <p className="py-4">{message}</p>
                <div className="flex justify-end space-x-4 mt-4">
                    <button
                        className="px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
                        onClick={onConfirm}
                    >
                        Bekræft
                    </button>
                    <button
                        className="px-4 py-2 bg-gray-300 text-black rounded-md hover:bg-gray-400"
                        onClick={onCancel}
                    >
                        Afbryd
                    </button>
                </div>
            </div>
        </dialog>
    );
}
