Feature: Create Order

  As a user, I want to create orders using GraphQL mutations, ensuring valid inputs create an order and invalid inputs result in appropriate errors.

  @Order
  Scenario: Successfully create an order
    Given a valid `createOrder` input
    When the `createOrder` GraphQL mutation is called
    Then the order should be successfully created

  @Order
  Scenario: Fail to create an order with invalid input
    Given an invalid `createOrder` input
    When the `createOrder` GraphQL mutation is called
    Then the order should not be created
    And an appropriate error message should be returned
