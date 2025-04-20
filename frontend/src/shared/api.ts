export const getAllProjects = async () => {
  const link = 'https://tulahack.eureka-team.ru/api/mock/MockedContest/projects'

  const result = await fetch("https://tulahack.eureka-team.ru/api/mock/MockedContest/projects", {
    "headers": {
      "accept": "text/plain",
      "accept-language": "ru-RU,ru;q=0.9,en-GB;q=0.8,en;q=0.7,en-US;q=0.6",
      "authorization": "Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJEaENfTFlqbjBlR3QxNlhPd1VDWk9IVDFPQzJLSDM4d2FUX2Y4MnJEUHg4In0.eyJleHAiOjE3NDUxNzAwOTgsImlhdCI6MTc0NTEzNDA5OCwiYXV0aF90aW1lIjoxNzQ1MTM0MDk4LCJqdGkiOiJmZWQ0N2UxMi02ZTI3LTQyNjQtODlmNy05MzZiNzAxNWJjNjIiLCJpc3MiOiJodHRwczovL2tleWNsb2FrLmV1cmVrYS10ZWFtLnJ1L3JlYWxtcy90dWxhaGFjayIsImF1ZCI6WyJ0dWxhaGFjay1jbGllbnQiLCJhY2NvdW50Il0sInN1YiI6ImE2M2NmYzhhLTEzNDgtNGEzYS05MjY5LWI0YjMyOTdiYWI0ZCIsInR5cCI6IkJlYXJlciIsImF6cCI6InR1bGFoYWNrLWNsaWVudCIsInNlc3Npb25fc3RhdGUiOiIwNjQ5MWViMy1iYzhkLTQwOTUtYWM5ZS1lZDg0NGI0ZTA3YjIiLCJhY3IiOiIxIiwiYWxsb3dlZC1vcmlnaW5zIjpbImh0dHBzOi8vbG9jYWxob3N0OjcwMDkvKiIsImh0dHBzOi8vdHVsYWhhY2suZXVyZWthLXRlYW0ucnUiLCJodHRwczovL2xvY2FsaG9zdDo1MDAxLyoiLCJodHRwOi8vMTI3LjAuMC4xOjMwMDgwLyoiLCJodHRwOi8vbG9jYWxob3N0OjUwMDAvKiJdLCJyZWFsbV9hY2Nlc3MiOnsicm9sZXMiOlsib2ZmbGluZV9hY2Nlc3MiLCJkZWZhdWx0LXJvbGVzLXR1bGFoYWNrIiwidW1hX2F1dGhvcml6YXRpb24iXX0sInJlc291cmNlX2FjY2VzcyI6eyJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im9wZW5pZCBlbWFpbCBwcm9maWxlIiwic2lkIjoiMDY0OTFlYjMtYmM4ZC00MDk1LWFjOWUtZWQ4NDRiNGUwN2IyIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsIm5hbWUiOiJTZXJnZXkgRXZzZWV2IiwicHJlZmVycmVkX3VzZXJuYW1lIjoic2VyZ2V5ZXZzZWV2IiwiZ2l2ZW5fbmFtZSI6IlNlcmdleSIsImZhbWlseV9uYW1lIjoiRXZzZWV2IiwiZW1haWwiOiJjb250ZXN0YW50QHR1bGFoYWNrLnJ1IiwiZ3JvdXAiOlsiL0NvbnRlc3RhbnRzIiwiL1B1YmxpYyJdfQ.bjl11760UZ2gLSRfA6xUPwSFm1PCxEeuVx1smiLoMt3fcPvvfpQAqP3SfL8hGD0FUGA1fcnhed3j808JsS4bJdGxbe2KwhDosTZLvGjbF-9FX5ibB6hAQR_02BrV2qJOh6zhYyGh7D2Qpr-qPkxiNfzF2TB-2A3hg0tMeHIFShPJy15tOSDTha8OEqNNZ1fHjacn-V4xbtRB7RTtKgqZ1-5NdRwagPWPKiUg_qe5bjFwuNyRslssC17fQo-NpDqxZILI5T89zK2V7rKhYTzNg1ItdDwRuUn4jfCbW1F6QT5z-WGamEn32DzV4ZJZCZwn1WutY_ZNcejQyp5N6V1UtA",
      "priority": "u=1, i",
      "sec-ch-ua": "\"Google Chrome\";v=\"135\", \"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"135\"",
      "sec-ch-ua-mobile": "?0",
      "sec-ch-ua-platform": "\"Windows\"",
      "sec-fetch-dest": "empty",
      "sec-fetch-mode": "cors",
      "sec-fetch-site": "same-origin"
    },
    "referrer": "https://tulahack.eureka-team.ru/api/swagger/index.html",
    "referrerPolicy": "strict-origin-when-cross-origin",
    "body": null,
    "method": "GET",
    "mode": "cors",
    "credentials": "include"
  });
  const projects = await result.json();
  return projects;
}


