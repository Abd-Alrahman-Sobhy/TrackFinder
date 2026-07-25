(() => {
    const assessmentRoot = document.querySelector('[data-assessment-root]');

    if (!assessmentRoot) {
        return;
    }

    const steps = Array.from(assessmentRoot.querySelectorAll('[data-step]'));
    const progressBar = assessmentRoot.querySelector('[data-progress-bar]');
    const questionCounter = assessmentRoot.querySelector('[data-question-counter]');
    const prevButton = assessmentRoot.querySelector('[data-prev-step]');
    const nextButton = assessmentRoot.querySelector('[data-next-step]');
    const submitButton = assessmentRoot.querySelector('[data-submit-step]');
    const form = assessmentRoot.querySelector('[data-assessment-form]');

    if (!steps.length || !progressBar || !questionCounter || !prevButton || !nextButton || !submitButton || !form) {
        return;
    }

    let currentStep = 0;
    const totalSteps = steps.length;

    const updateUi = () => {
        steps.forEach((step, index) => {
            step.classList.toggle('d-none', index !== currentStep);
            step.classList.toggle('is-active', index === currentStep);
        });

        const progress = totalSteps === 0 ? 0 : ((currentStep + 1) / totalSteps) * 100;
        progressBar.style.width = `${progress}%`;
        questionCounter.textContent = `Question ${currentStep + 1} of ${totalSteps}`;
        prevButton.disabled = currentStep === 0;
        nextButton.classList.toggle('d-none', currentStep === totalSteps - 1);
        submitButton.classList.toggle('d-none', currentStep !== totalSteps - 1);
    };

    nextButton.addEventListener('click', () => {
        if (currentStep < totalSteps - 1) {
            currentStep += 1;
            updateUi();
        }
    });

    prevButton.addEventListener('click', () => {
        if (currentStep > 0) {
            currentStep -= 1;
            updateUi();
        }
    });

    form.addEventListener('submit', (event) => {
        const apiUrl = form.dataset.apiUrl;

        if (!apiUrl) {
            return;
        }

        event.preventDefault();

        const payload = {
            userId: form.querySelector('[name="UserId"]')?.value ?? '',
            answers: steps.map((step, index) => {
                const questionIdInput = step.querySelector(`[name="Answers[${index}].QuestionId"]`);
                const selectedOption = step.querySelector(`input[name="Answers[${index}].OptionNumber"]:checked`);

                return {
                    questionId: questionIdInput ? parseInt(questionIdInput.value, 10) : 0,
                    optionNumber: selectedOption ? parseInt(selectedOption.value, 10) : 0
                };
            })
        };

        if (!payload.userId) {
            form.insertAdjacentHTML('afterbegin', '<div class="alert alert-brand">TODO: populate the UserId before submitting the assessment.</div>');
            return;
        }

        if (payload.answers.some((answer) => answer.optionNumber === 0)) {
            form.insertAdjacentHTML('afterbegin', '<div class="alert alert-brand">Please answer every question before submitting the assessment.</div>');
            return;
        }

        fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })
            .then((response) => {
                if (!response.ok) {
                    throw new Error('Assessment submission failed.');
                }

                return response.json();
            })
            .then((result) => {
                if (result?.assessmentResultId) {
                    window.location.href = `/Assessment/Result?id=${encodeURIComponent(result.assessmentResultId)}`;
                }
            })
            .catch(() => {
                form.insertAdjacentHTML('afterbegin', '<div class="alert alert-brand">Assessment submission failed. Check the API route and payload shape.</div>');
            });
    });

    updateUi();
})();
