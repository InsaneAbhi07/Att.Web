    let currentPage = 1;

	const searchBox = document.getElementById("searchBox");
	const pageSize = document.getElementById("pageSize");
	const table = document.getElementById("deptTable");
	const tbody = table.getElementsByTagName("tbody")[0];
	const rows = Array.from(tbody.getElementsByTagName("tr"));
	const pagination = document.getElementById("pagination");

	function renderTable() {
		let search = searchBox.value.toLowerCase();
		let limit = parseInt(pageSize.value);

		let filtered = rows.filter(r =>
			r.innerText.toLowerCase().includes(search)
		);

		let totalPages = Math.ceil(filtered.length / limit);
		if (currentPage > totalPages) currentPage = 1;

		// hide all
		rows.forEach(r => r.style.display = "none");

		let start = (currentPage - 1) * limit;
		let end = start + limit;

		filtered.slice(start, end).forEach(r => {
			r.style.display = "";
		});

		renderPagination(totalPages);
	}

	function renderPagination(totalPages) {
		pagination.innerHTML = "";

		for (let i = 1; i <= totalPages; i++) {
			let li = document.createElement("li");
			li.className = "page-item " + (i === currentPage ? "active" : "");

			let a = document.createElement("a");
			a.className = "page-link";
			a.href = "#";
			a.innerText = i;

			a.onclick = function (e) {
				e.preventDefault();
				currentPage = i;
				renderTable();
			};

			li.appendChild(a);
			pagination.appendChild(li);
		}
	}

	searchBox.addEventListener("keyup", () => {
		currentPage = 1;
		renderTable();
	});

	pageSize.addEventListener("change", () => {
		currentPage = 1;
		renderTable();
	});

	renderTable();
