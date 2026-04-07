document.addEventListener('DOMContentLoaded', function () {
	console.log('DOM carregado, inicializando sidebar toggle...');

	const sidebar = document.getElementById("sidebar");
	const toggleButton = document.getElementById("toggle-sidebar");
	const nav = document.getElementById("navLeft");
	const main = document.querySelector(".main");

	console.log('Elementos encontrados:', {
		sidebar: !!sidebar,
		toggleButton: !!toggleButton,
		nav: !!nav,
		main: !!main
	});

	if (toggleButton && sidebar && nav && main) {
		console.log('Adicionando event listener...');
		toggleButton.addEventListener("click", function (e) {
			console.log('Botão clicado!');
			e.preventDefault();

			sidebar.classList.toggle("collapsed");

			if (sidebar.classList.contains("collapsed")) {
				// Sidebar minimizada - esconder menu e ajustar main
				nav.style.display = 'none';
				main.style.marginLeft = '60px';
				console.log('Sidebar minimizada');
			} else {
				// Sidebar expandida - mostrar menu e ajustar main
				nav.style.display = '';
				main.style.marginLeft = '250px';
				console.log('Sidebar expandida');
			}
		});
	} else {
		console.error('Elementos não encontrados:', {
			sidebar: sidebar,
			toggleButton: toggleButton,
			nav: nav,
			main: main
		});
	}
});

