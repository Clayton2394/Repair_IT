window.repairIT = {
    /** Télécharge un flux .NET sous forme de fichier dans le navigateur. */
    telechargerFichier: async function (nomFichier, typeMime, fluxDotNet) {
        const contenu = await fluxDotNet.arrayBuffer();
        const blob = new Blob([contenu], { type: typeMime });
        const url = URL.createObjectURL(blob);

        const lien = document.createElement("a");
        lien.href = url;
        lien.download = nomFichier;
        document.body.appendChild(lien);
        lien.click();
        lien.remove();

        URL.revokeObjectURL(url);
    }
};
