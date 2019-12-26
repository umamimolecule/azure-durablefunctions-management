# Changelog

<a name="1.1.1" />

## [1.1.1] - 2019-12-25

### Changed
 - Fixed bug where management functions were not being included when function project is deployed to Azure.
 - Minor tweaks to readme file.

<a name="1.1.0" />

## [1.1.0] - 2019-12-25

### Added
 - Added test settings file to collect code coverage.

### Changed
 - PurgeInstanceHistoryForCondition now validates status filters to only allow `Completed`, `Terminated` or `Failed`.
 - PurgeInstanceHistoryForCondition and PurgeInstanceHistory now return `200 OK` response instead of `202 Accepted` and also returns a payload indicated how many instances were purged.
 - RewindInstance and TerminateInstance now handle when instance is not in a valid state to be processed, and now return `400 Bad Request` when this happens instead of returning `500 Internal Server Error`.
 - Update documentation.
 - Add more tests.

<a name="1.0.1" />

## [1.0.1] - 2019-12-24

### Added
 - Documentation for endpoints.
 - Added readme file.

### Changed
 - Fixed bug where instance status properties were being serialized as integers instead of strings.

<a name="1.0.0" />

## [1.0.0] - 2019-12-24
 - Initial release.