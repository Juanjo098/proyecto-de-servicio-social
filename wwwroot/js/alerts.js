const confirmar = (titulo, texto, accion, resultado, form) => {
    return (
        Swal.fire({
            title: titulo,
            text: texto,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: accion
        }).then((result) => {
            if (result.isConfirmed) {
                Swal.fire(
                    resultado,
                    '',
                    'success'
                )
                form.submit()
            }
        })
    )
}

const excepcion = () => {
    return (
        Swal.fire({
            icon: 'error',
            title: 'Error...',
            text: 'Algo salio mal'
        })
    )
}